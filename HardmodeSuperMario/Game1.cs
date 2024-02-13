using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using static MagicNumbers.Screen;

namespace Project;

public enum GameState
{
    Initialized,
    LoadingScreen,
    GameOver,
    Running,
    PlayingAnimation, // When Mario gets a powerup, takes damage or dies
    Paused
    // TODO: Add more states (such as level transition, game over screen, etc)
}

public class Game1 : Game
{
    public const string LevelName = "1-1";

    public static Game1 Instance { get; private set; }
    public static bool HardMode { get; private set; }

    public LevelObjectManager Level { get; private set; }
    public Color Background { get; set; }

    private GameState state;
    private GameState stateWas;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Matrix spriteScale;
    private IController[] controllers;
    private LevelLoader[] normalLevels;
    private LevelLoader[] hardLevels;

    public Game1()
    {
        Instance = this;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void UpdateState(GameState newState, Entity animation = null)
    {
        switch (state, newState)
        {
            case (_, GameState.GameOver):
                Sound.BackgroundMusic = Sound.GameOver;
                break;

            case (_, GameState.LoadingScreen):
                // Stop music, reset loading screen and load first level
                Mario.Instance.BecomeVulnerable();
                Sound.BackgroundMusic.Stop(true);
                LoadingScreen.ResetTimer();
                HUD.Instance.ResetTimer();
                break;

            case (GameState.LoadingScreen, GameState.Running):
                WarpTo(0);
                break;

            case (GameState.PlayingAnimation, GameState.Running):
                Level.MarioDecorator = Mario.Instance;
                break;

            case (GameState.Running, GameState.PlayingAnimation):
                Level.MarioDecorator = animation;
                break;

            case ( >= GameState.Running, GameState.Paused):
                // Save current state, pause music and play pause noise
                stateWas = state;
                Sound.BackgroundMusic.Pause();
                Sound.Pause.Play();
                break;

            case (GameState.Paused, >= GameState.Running):
                // Unpause music and play pause noise
                Sound.BackgroundMusic.Resume();
                Sound.Pause.Play();
                break;

            default:
                return;
        }

        state = newState;
    }

    public void WarpTo(int level, Vector2? position = null)
    {
        Level = (HardMode ? hardLevels : normalLevels)[level].LoadFromFile();
        if (position is Vector2 pos) Mario.Instance.Position = pos;
        // Move mario one pixel down, so he doesn't fall for a single frame
        Mario.Instance.Position += Vector2.UnitY;
        Mario.Instance.Velocity = Vector2.Zero;
        Mario.Instance.WarpPipeVeclocity = null;
        Level.ReloadScreen();
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = ScaledWidth;
        graphics.PreferredBackBufferHeight = ScaledHeight;
        graphics.ApplyChanges();

        controllers = new[] { MouseInput() };
        normalLevels = new[] 
        {
            new LevelLoader("1-1"),
            new LevelLoader("1-1Under"),
        };
        hardLevels = new[] 
        { 
            new LevelLoader("Lost1-1"),
            new LevelLoader("Lost1-1Under"),
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create the scale transform for Draw. 
        // Do not scale the sprite depth (Z=1).
        spriteScale = Matrix.CreateScale(SizeMult, SizeMult, 1);
        Sound.LoadContent(Content);
        TextureStorage.Instance.LoadContent(Content);
        StartGame(false);
    }

    protected override void Update(GameTime gameTime)
    {
        if (state == GameState.GameOver)
        {
            GameOver.Update();
            return;
        }

        KeyboardController.UpdateState();
        if (KeyboardController.IsQuitting()) Exit();
        else if (KeyboardController.IsRestarting()) StartGame(HardMode);
        else if (KeyboardController.IsPausing()) TogglePause();

        switch (state)
        {
            case GameState.LoadingScreen:
                FrameRule.Update();
                LoadingScreen.Update();
                break;
            case GameState.Running:
                FrameRule.Update();
                HUD.Instance.Update();
                Level.Update();
                BlockAnimation.Update();
                break;

            // Everything but Mario is frozen
            case GameState.PlayingAnimation:
                Level.MarioDecorator.Update();
                BlockAnimation.Update();
                break;

            case GameState.Paused:
                return;
        }

        for (int i = 0; i < controllers.Length; i++)
            controllers[i].Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        bool loading = state is GameState.LoadingScreen or GameState.GameOver;
        GraphicsDevice.Clear(loading ? Color.Black : Background);

        if (!loading)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.BackToFront,
                transformMatrix: Level.ScreenOffset * spriteScale,
                samplerState: SamplerState.PointClamp
            );
            Level.Draw(spriteBatch);
            spriteBatch.End();
        }

        spriteBatch.Begin(
            transformMatrix: spriteScale,
            samplerState: SamplerState.PointClamp
        );

        switch (state)
        {
            case GameState.GameOver:
                GameOver.Draw(spriteBatch);
                break;
            case GameState.LoadingScreen:
                LoadingScreen.Draw(spriteBatch);
                HUD.Instance.Draw(spriteBatch);
                break;
            default:
                HUD.Instance.Draw(spriteBatch);
                break;
        }

        spriteBatch.End();

        base.Draw(gameTime);
    }

    private IController MouseInput()
    {
        var controller = new MouseController();
        controller.LeftClick += () => StartGame(false);
        controller.RightClick += () => StartGame(true);
        return controller;
    }

    private void TogglePause()
    {
        if (Sound.Pause.State == SoundState.Playing) return;

        switch (state)
        {
            case GameState.Paused:
                UpdateState(stateWas);
                break;
            case >= GameState.Running:
                stateWas = state;
                UpdateState(GameState.Paused);
                break;
        }
    }

    private void StartGame(bool hard)
    {
        HardMode = hard;
        Mario.Lives = MagicNumbers.Mario.StartLives;
        UpdateState(GameState.LoadingScreen);
    }
}
