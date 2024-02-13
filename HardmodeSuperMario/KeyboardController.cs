using Microsoft.Xna.Framework.Input;

namespace Project;

public static class KeyboardController
{
    public enum Command
    {
        Quit,
        Restart,
        Pause,
        MoveLeft,
        MoveRight,
        Jump,
        Crouch,
        Fireball,
        MakeMarioSuper,
        MakeMarioFire,
        MakeMarioStar,
        DamageMario
    }

    private static KeyboardState lastState, currentState = Keyboard.GetState();
    private static readonly (Keys, Keys?)[] commandMapping = new (Keys, Keys?)[12];

    static KeyboardController()
    {
        commandMapping[(int)Command.Quit] = (Keys.Q, null);
        commandMapping[(int)Command.Restart] = (Keys.R, null);
        commandMapping[(int)Command.Pause] = (Keys.P, null);
        commandMapping[(int)Command.MoveLeft] = (Keys.A, Keys.Left);
        commandMapping[(int)Command.MoveRight] = (Keys.D, Keys.Right);
        commandMapping[(int)Command.Jump] = (Keys.W, Keys.Up);
        commandMapping[(int)Command.Crouch] = (Keys.S, Keys.Down);
        commandMapping[(int)Command.Fireball] = (Keys.Z, Keys.N);
        commandMapping[(int)Command.MakeMarioSuper] = (Keys.D1, null);
        commandMapping[(int)Command.MakeMarioFire] = (Keys.D2, null);
        commandMapping[(int)Command.MakeMarioStar] = (Keys.D3, null);
        commandMapping[(int)Command.DamageMario] = (Keys.E, null);
    }

    public static void UpdateState()
    {
        lastState = currentState;
        currentState = Keyboard.GetState();
    }

    public static bool IsQuitting() => IsPressed(Command.Quit);
    public static bool IsRestarting() => IsDown(Command.Restart);
    public static bool IsPausing() => IsDown(Command.Pause);
    public static bool IsMovingLeft() => IsPressed(Command.MoveLeft);
    public static bool IsMovingRight() => IsPressed(Command.MoveRight);
    public static bool IsJumping() => IsDown(Command.Jump);
    public static bool IsHoldingJump() => IsPressed(Command.Jump);
    public static bool IsCrouching() => IsPressed(Command.Crouch);
    public static bool IsThrowingFireball() => IsDown(Command.Fireball);
    public static bool MakeMarioSuper() => IsDown(Command.MakeMarioSuper);
    public static bool MakeMarioFire() => IsDown(Command.MakeMarioFire);
    public static bool MakeMarioStar() => IsDown(Command.MakeMarioStar);
    public static bool DamageMario() => IsDown(Command.DamageMario);

    private static bool IsPressed(Command c)
    {
        var (key1, key2) = commandMapping[(int)c];
        return currentState.IsKeyDown(key1)
            || (key2 is Keys key && currentState.IsKeyDown(key));
    }

    private static bool IsDown(Command c)
    {
        var (key1, key2) = commandMapping[(int)c];
        return (currentState.IsKeyDown(key1) && lastState.IsKeyUp(key1))
            || (key2 is Keys key && currentState.IsKeyDown(key) && lastState.IsKeyUp(key));
    }
}
