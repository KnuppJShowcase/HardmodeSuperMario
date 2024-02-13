using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static MagicNumbers.Entity;

namespace Project;

// Abstract class containing common code for dynamic sprites
public abstract class Entity : ISprite
{
    // Bounding box used for collision with the level geometry
    public virtual Rectangle BoundingBox => Rectangle.Empty;

    // Hitbox is used to check for collision between two Entities.
    // In most cases, it should be smaller than the bounding box
    public virtual Rectangle Hitbox => Rectangle.Empty;

    public bool Despawned { get; set; } = false;

    // Position and Velocity -- self explanitory
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; } = Vector2.Zero;

    // If this is false, we need to apply constant acceleration in Update()
    protected virtual bool OnGround { get; set; } = false;
    protected bool EjectedFromWall { get; set; } = false;

    public Entity() { }
    public Entity(int x, int y) => Position = ISprite.Size * new Vector2(x, y);


    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void Update()
    {
        if (!OnGround)
            Velocity = Velocity with { Y = Math.Min(Velocity.Y + GForce, TerminalVelocity) };
        Position += Velocity;
    }

    // Called when we intersect with a block
    public virtual void HitGround(Collision direction, Point size)
    {
        switch (direction)
        {
            case Collision.Down when !OnGround:
                // Push out of ground, reset Y velocity and set flag
                Position = Position with { Y = MathF.Floor(Position.Y - size.Y + 1) };
                if (Velocity.Y > 0) Velocity = Velocity with { Y = 0 };
                OnGround = true;
                break;
            case Collision.Up:
                // Push out of ground and reset Y velocity
                Position += new Vector2(0, size.Y);
                if (Velocity.Y < 0) Velocity = Velocity with { Y = 0 };
                break;
            case Collision.Left or Collision.Right when !EjectedFromWall:
                // Push left/right out of block
                Position = Position with { X = MathF.Floor(Position.X + (direction == Collision.Left ? size.X : -size.X)) };
                EjectedFromWall = true;
                break;
        }
    }

    public void ResetOnGround()
    {
        if (OnGround || Velocity.Y < 0)
            EjectedFromWall = false;
        OnGround = false;
    }

    // Called when this collides a sprite that doesn't have an explict method assigned
    public virtual void DefaultCollision(Collision direction) { }

    public virtual void Bounce(bool right) => Velocity = new(right ? 2 : -2, -4);
}
