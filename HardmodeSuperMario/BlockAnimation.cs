namespace Project;

public static class BlockAnimation
{
    const int FramesPerTexture = 8;
    const int TotalFrames = 48;
    static readonly int[] TextureCycle = new[]
        { 0, 0, 1, 2, 1, 0 };

    private static int frameCount = 0;
    public static int TextureOffset
        => TextureCycle[frameCount / FramesPerTexture];

    public static void Update()
        => frameCount = (frameCount + 1) % TotalFrames;
}
