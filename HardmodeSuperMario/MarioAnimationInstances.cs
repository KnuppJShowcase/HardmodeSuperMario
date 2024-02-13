namespace Project;

public partial class MarioAnimation
{
    public static MarioAnimation SmallToSuper { get; } = new(
        new[]
        {
            (6, 2, 1),
            (14, 0, 2),
            (6, 2, 1),
            (14, 0, 2),
            (6, 2, 1),
            (14, 0, 2),
            (6, 2, 1),
            (14, 0, 2),
            (6, 0, 2),
            (6, 2, 1),
            (14, 0, 2),
            (6, 0, 2)
        }
    );

    public static MarioAnimation SuperToSmall { get; } = new(
        new[]
        {
            (4, 0, 2),
            (4, 0, 2),
            (4, 0, 2),
            (4, 0, 2),
            (10, 2, 1),
            (10, 0, 2),
            (10, 2, 1),
            (10, 0, 2),
            (10, 2, 1),
            (10, 0, 2),
            (10, 2, 1),
            (10, 0, 2),
            (10, 2, 1),
            (10, 0, 2),
        }
    )
    { flicker = true };
}
