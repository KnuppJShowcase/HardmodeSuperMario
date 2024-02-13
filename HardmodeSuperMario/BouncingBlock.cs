using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class BouncingBlock : IGeometry
{
    public Block Base { get; init; }
    public Entity ToSpawn { get; init; }

    private float yOffset = 0;
    private float yVelocity = -2.0f;

    public IGeometry Update()
    {
        if (yOffset >= 1)
        {
            if (ToSpawn != null)
                Game1.Instance.Level.Load(ToSpawn);

            return Base;
        }

        yOffset = Math.Min(yOffset + yVelocity, 1);
        yVelocity += 0.5f;
        return this;
    }

    public IGeometry CollideWith(Entity e, Collision direction, Rectangle intersection)
    {
        if (direction == Collision.Down)
            e.Bounce(intersection.Center.X % ISprite.Size > ISprite.Size / 2);
        else
            e.HitGround(direction, intersection.Size);
        return this;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
        => Base.Draw(spriteBatch, position + new Vector2(0, MathF.Floor(yOffset)));
}
