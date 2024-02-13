using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public interface IGeometry
{

    IGeometry Update() => this;
    void Draw(SpriteBatch spriteBatch, Vector2 position);
    IGeometry CollideWith(Entity e, Collision direction, Rectangle size) => this;
}
