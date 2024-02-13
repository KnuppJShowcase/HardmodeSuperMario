using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class CommandBlock : IGeometry
{
    public static CommandBlock StopScroll { get; } = new(() => Game1.Instance.Level.MoveScreen = false);
    private Command cmd;

    private CommandBlock(Command cmd) => this.cmd = cmd;

    public IGeometry Update()
    {
        cmd();
        return null;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position) { }
}
