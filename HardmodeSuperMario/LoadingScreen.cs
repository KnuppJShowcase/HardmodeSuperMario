using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public static class LoadingScreen
{
    private static readonly SpriteFont font = TextureStorage.Instance.GetHUDFont();
    private static readonly Texture2D texture = TextureStorage.Instance.GetMarioSheet();

    private static int timer;

    public static void ResetTimer() => timer = 7;

    public static void Update()
    {
        if (FrameRule.IsZero && --timer == 0)
            Game1.Instance.UpdateState(GameState.Running);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            font, "World 1-1",
            new Vector2(96, 68), Color.White
        );
        // TODO: Magic numbers
        spriteBatch.Draw(
            texture, new Vector2(96, 93),
            new Rectangle(6 * ISprite.Size, 2 * ISprite.Size, ISprite.Size, ISprite.Size),
            Color.White
        );
        spriteBatch.DrawString(
            font, string.Format("×{0,3}", Mario.Lives),
            new Vector2(128, 100), Color.White
        );
    }
}

public static class GameOver
{
    private static readonly SpriteFont font = TextureStorage.Instance.GetHUDFont();
    private static int timer = 60;

    public static void Update()
    {
        if (Sound.GameOver.State == SoundState.Stopped
            && --timer == 0)
            Game1.Instance.Exit();
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(
            font, "Game Over",
            new Vector2(96, 108), Color.White
        );
    }
}