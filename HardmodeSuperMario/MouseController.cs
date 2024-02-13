using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Project;

public class MouseController : IController
{
    // List of keys that were pressed in the previous frame
    private List<Keys> lastPressed = new();

    private ButtonState LastLeftClick;
    private ButtonState LastRightClick;
    public event Command LeftClick;
    public event Command RightClick;

    public void Update()
    {
        MouseState state = Mouse.GetState();
        if (state.LeftButton == ButtonState.Pressed && !(state.LeftButton == LastLeftClick))
            LeftClick?.Invoke();
        else if (state.RightButton == ButtonState.Pressed && !(state.RightButton == LastRightClick))
            RightClick?.Invoke();

        LastLeftClick = state.LeftButton;
        LastRightClick = state.RightButton;
    }
}


        
