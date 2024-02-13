using Microsoft.Xna.Framework.Graphics;

namespace Project;

public interface ISprite
{
    public const int Size = 16;

    void Update() { }
    void Draw(SpriteBatch spriteBatch);

    // Called when this collides with a sprite that doesn't have an explict method assigned
    void DefaultCollision(Collision direction) { }
}
