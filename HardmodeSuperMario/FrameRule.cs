namespace Project;

public static class FrameRule
{
    const int TotalFrames = 21;

    private static int timer = -1;
    public static bool IsZero => timer == 0;

    public static void Update() => timer = (timer + 1) % TotalFrames;
}
