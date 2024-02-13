using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class CoinEntity : Entity
{
    private int XCoord, YCoord;
    private static Texture2D Texture = TextureStorage.Instance.GetPowerUpSheet();
    private float initalPosition;
    int counter;

    public CoinEntity(int x, int y) : base(x, y)
    {
        initalPosition = Position.Y;
        XCoord = 0;
        YCoord = 5 * ISprite.Size;
        Velocity = new Vector2(0, -4);
    }

    public override void Update()
    {
        if (++counter > 5)
        {
            counter = 0;
            XCoord = (XCoord + ISprite.Size) % 64;
        }
        base.Update();

        if (Position.Y >= initalPosition)
            Despawned = true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        Rectangle sourceRectangle = new Rectangle(XCoord, YCoord, ISprite.Size, ISprite.Size);
        spriteBatch.Draw(
            texture: Texture,
            position: Vector2.Floor(Position),
            sourceRectangle,
            color: Color.White
        );
    }
}
