using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class WarpPipe : IGeometry
{
    public bool Horizontal { get; init; } = false;
    public int Room { get; init; } = 0;
    public Vector2? Position { get; init; } = null;
    public Block Base { get; init; }

    private int animationFrames = 0;

    public IGeometry Update()
    {
        if (animationFrames > 0 && FrameRule.IsZero && --animationFrames == 0)
            Game1.Instance.WarpTo(Room, Position);
        return this;
    }

    public IGeometry CollideWith(Entity e, Collision direction, Rectangle intersection)
    {
        e.HitGround(direction, intersection.Size);

        if (e is Mario mario && (Horizontal
            ? KeyboardController.IsMovingRight() && intersection.Bottom % ISprite.Size == 0
            : KeyboardController.IsCrouching() && intersection.Left % ISprite.Size is >= 4 and <= 12))
        {
            Sound.BackgroundMusic = Sound.Pipe;
            animationFrames = 3;
            Mario.Instance.WarpPipeVeclocity = Horizontal ? Vector2.UnitX : Vector2.UnitY;
        }

        return this;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
        => Base.Draw(spriteBatch, position);
}

