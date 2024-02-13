using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public partial class MarioAnimation : Entity
{
    const int FramesPerTexture = 4;

    private static readonly Texture2D texture = TextureStorage.Instance.GetMarioSheet();
    private readonly Rectangle[] sources;
    private bool flicker = false;
    private float layer = 0;

    private int index;
    private int framesLeft;
    private SpriteEffects effects;

    private MarioAnimation((int x, int y, int h)[] sources)
    {
        this.sources = Array.ConvertAll(sources, static tuple => new Rectangle(
            tuple.x * ISprite.Size,
            tuple.y * ISprite.Size,
            ISprite.Size,
            tuple.h * ISprite.Size
        ));
    }

    public override void Update()
    {
        if (--framesLeft == 0)
        {
            framesLeft = 4;
            if (++index == sources.Length)
            {
                index = 0;
                Game1.Instance.UpdateState(GameState.Running);
                return;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (flicker && framesLeft % 2 == 1) return;

        spriteBatch.Draw(
            texture,
            Position,
            sources[index],
            Color.White,
            rotation: 0f,
            origin: new Vector2(0, sources[index].Height - ISprite.Size),
            scale: 1f,
            effects,
            layerDepth: layer
        );
    }

    public void PlayAt(Vector2 position, SpriteEffects effects)
    {
        index = 0;
        framesLeft = 4;
        Position = position;
        this.effects = effects;
        Game1.Instance.UpdateState(GameState.PlayingAnimation, this);
    }
}
