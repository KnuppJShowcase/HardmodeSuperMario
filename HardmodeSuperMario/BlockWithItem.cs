using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class BlockWithItem : IGeometry
{
    public enum Item
    {
        Coin,
        Progressive,
        Star,
        OneUp,
        Poison
        // Coins,
    }

    public Item Type { get; init; }
    public Block Base { get; init; }

    public IGeometry CollideWith(Entity e, Collision direction, Rectangle intersection)
    {
        e.HitGround(direction, intersection.Size);

        if (direction == Collision.Up && e is Mario mario)
        {
            int x = intersection.X / ISprite.Size, y = intersection.Y / ISprite.Size;
            if (Type == Item.Coin)
            {
                HUD.Instance.AddCoin();
                Game1.Instance.Level.Load(new CoinEntity(x, y - 1));
            }
            else
                Sound.PowerUpSpawn.Play();

            return new BouncingBlock
            {
                Base = Block.Empty,
                ToSpawn = Type switch
                {
                    Item.Progressive when mario.Powerup == Mario.Power.Small
                        => new PowerUp(x, y, PowerUp.Sprite.SuperMushroom),
                    Item.Progressive
                        => new PowerUp(x, y, PowerUp.Sprite.FireFlower),
                    Item.Star
                        => new PowerUp(x, y, PowerUp.Sprite.SuperStar),
                    Item.OneUp
                        => new PowerUp(x, y, PowerUp.Sprite.OneUpMushroom),
                    Item.Poison
                        => new PowerUp(x, y, PowerUp.Sprite.PoisonMushroom),
                    _ => null
                }
            };
        }

        return this;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
        => Base.Draw(spriteBatch, position);
}

public class InvisibleBlock : BlockWithItem, IGeometry
{
    IGeometry IGeometry.CollideWith(Entity e, Collision direction, Rectangle size)
    {
        if (direction == Collision.Up && e is Mario mario)
            return CollideWith(e, direction, size);

        return this;
    }

    void IGeometry.Draw(SpriteBatch _, Vector2 __) { }
}
