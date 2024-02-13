using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class DeadMario : Entity
{
    public static DeadMario Instance { get; } = new(5, 2);

    private static readonly Texture2D texture = TextureStorage.Instance.GetMarioSheet();
    private readonly Rectangle source;

    private bool onScreen;
    private int timer;

    private DeadMario(int x, int y)
        => source = new Rectangle(
            x * ISprite.Size,
            y * ISprite.Size,
            ISprite.Size,
            ISprite.Size
        );

    public override void Update()
    {
        Position += Velocity;
        Velocity += new Vector2(0, 0.2f);
        if (Sound.MarioDie.State == SoundState.Stopped && --timer == 0)
        {
            if (Mario.Lives > 0)
                Game1.Instance.UpdateState(GameState.LoadingScreen);
            else
                Game1.Instance.UpdateState(GameState.GameOver);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (onScreen)
            spriteBatch.Draw(texture, Position, source, Color.White);
    }

    public void PlayAt(Vector2 position, bool onScreen)
    {
        this.onScreen = onScreen;
        timer = 30;
        Position = position;
        Velocity = new Vector2(0, -4);
        Sound.BackgroundMusic = Sound.MarioDie;
        Game1.Instance.UpdateState(GameState.PlayingAnimation, this);
    }
}
