using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public partial class Block : IGeometry
{
    private static readonly Texture2D blocks = TextureStorage.Instance.GetBlockSheet();

    private Rectangle source;
    private bool animated;
    private float layer = 1;

    private Block(int x, int y) =>
        source = new Rectangle(x * ISprite.Size, y * ISprite.Size, ISprite.Size, ISprite.Size);

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
        => spriteBatch.Draw(
            texture: blocks,
            position,
            sourceRectangle: animated
                ? source with
                { X = source.X + ISprite.Size * BlockAnimation.TextureOffset }
                : source,
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1,
            effects: SpriteEffects.None,
            layerDepth: layer
        );


    private class CoinBlock : Block, IGeometry
    {
        public CoinBlock(int x, int y) : base(x, y) { }

        public IGeometry CollideWith(Entity e, Collision _, Rectangle __)
        {
            if (e is Mario)
            {
                HUD.Instance.AddCoin();
                return null;
            }

            return this;
        }
    }

    private class FlagPole : Block, IGeometry
    {
        public FlagPole(int x, int y) : base(x, y) { }

        public IGeometry CollideWith(Entity e, Collision _, Rectangle __)
        {
            if (e is Mario mario)
            {
                mario.grabFlagDie();
            }

            return this;
        }
    }

    private class SolidBlock : Block, IGeometry
    {
        public SolidBlock(int x, int y) : base(x, y)
            => layer = 0.5f;

        public IGeometry CollideWith(Entity e, Collision direction, Rectangle intersection)
        {
            e.HitGround(direction, intersection.Size);
            return this;
        }
    }

    private class BreakableBlock : SolidBlock, IGeometry
    {
        public BreakableBlock(int x, int y) : base(x, y) { }

        IGeometry IGeometry.CollideWith(Entity e, Collision direction, Rectangle intersection)
        {
            e.HitGround(direction, intersection.Size);
            switch (e, direction)
            {
                case (Mario { Powerup: Mario.Power.Small }, Collision.Up):
                    Sound.Bump.Play();
                    return new BouncingBlock { Base = this };

                case (Mario, Collision.Up):
                    HUD.Instance.BrickBreak();
                    return null;

                default:
                    return this;
            }
        }
    }
}
