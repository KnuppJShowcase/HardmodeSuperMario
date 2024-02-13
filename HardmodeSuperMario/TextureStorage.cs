using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project;
public class TextureStorage
{
    private static readonly TextureStorage instance = new TextureStorage();
    public static TextureStorage Instance
    {
        get
        {
            return instance;
        }
    }
    private TextureStorage() { }
    //SS == SpriteSheet
    protected Texture2D BlockBrickSpritesSS;
    protected Texture2D EnemiesSS;
    protected Texture2D FireballSS;
    protected Texture2D MarioSS;
    protected Texture2D PowerUpSS;
    protected SpriteFont HUDFont;
    public void LoadContent(ContentManager Content)
    {
        BlockBrickSpritesSS = Content.Load<Texture2D>("BlockBrickSprites");
        EnemiesSS = Content.Load<Texture2D>("enemies");
        FireballSS = Content.Load<Texture2D>("Fireball");
        MarioSS = Content.Load<Texture2D>("mario");
        PowerUpSS = Content.Load<Texture2D>("PowerUps");
        HUDFont = Content.Load<SpriteFont>("HUD");
    }

    public Texture2D GetBlockSheet()
    {
        return BlockBrickSpritesSS;
    }
    public Texture2D GetEnemySheet()
    {
        return EnemiesSS;
    }
    public Texture2D GetFireballSheet()
    {
        return FireballSS;
    }
    public Texture2D GetMarioSheet()
    {
        return MarioSS;
    }
    public Texture2D GetPowerUpSheet()
    {
        return PowerUpSS;
    }

    public SpriteFont GetHUDFont()
    {
        return HUDFont;
    }
}