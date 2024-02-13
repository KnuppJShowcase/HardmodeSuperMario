namespace Project;

public partial class Block
{
    public static Block Ground { get; } = new SolidBlock(0, 0);
    public static Block Ground2 { get; } = new SolidBlock(15, 0);
    public static Block BlueGround { get; } = new SolidBlock(0, 2);
    public static Block Stair { get; } = new SolidBlock(0, 1);
    public static Block[,] Hill { get; } = new Block[,]
    {
        { null,      null,       new(8, 8) },
        { null,      new(8, 8),  new(8, 9) },
        { new(9, 8), new(8, 9),  new(9, 9) },
        { null,      new(10, 8), new(10, 9) },
        { null,      null,       new(10, 8) }
    };
    public static Block Brick { get; } = new BreakableBlock(1, 0);
    public static Block Brick2 { get; } = new BreakableBlock(17, 0);
    public static Block BlueBrick { get; } = new BreakableBlock(2, 2);
    public static Block QuestionMark { get; } = new SolidBlock(23, 0) { animated = true };
    public static Block Empty { get; } = new SolidBlock(26, 0);
    public static Block Coin { get; } = new CoinBlock(23, 3) { animated = true };
    public static Block HardBlock { get; } = new SolidBlock(0, 1);
    public static Block[] Pipe { get; } = new SolidBlock[]
    {
        new(0,8), new(1,8), new(0,9), new(1,9)
    };
    public static Block[] HorizontalPipe { get; } = new SolidBlock[]
    {
        new(2,8), new(2,9), new(3,8), new(3,9), new(4,8), new(4,9), new(0,9)
    };
    public static Block[] Pole { get; } = new FlagPole[]
    {
        new(16,8), new(16,9)
    };
    public static Block[] Bush { get; } = new Block[]
    {
        new(11,9), new(12,9), new(13,9)
    };
    public static Block[] Mushroom { get; } = new Block[]
    {
        new(16,0)
    };
    public static Block[] Tree { get; } = new Block[]
    {
        new(7,1), new(17,9), new(18,9), new(18,8)
    };
    public static Block[,] Cloud { get; } = new Block[,]
    {
        {new(0,20), new(1,20), new(2,20)},
        {new(0,21), new(1,21), new(2,21)}
    };
    public static Block[,] SmileCloud { get; } = new Block[,]
    {
        {new(5,20), new(6,20), new(7,20)},
        {new(5,21), new(6,21), new(7,21)}
    };
    /*
    public static Block[,] Castle { get; } = new Block[,]
    {
        {null,      new(10,0), new(10,0), new(10,0), null     },
        {null,      new(11,0), new(12,0), new(13,0), null     },
        {null,      new(11,0), new(12,0), new(13,0), null     },
        {null,      new(0,20), new(1,20), new(2,20), null     },
        {new(10,0), new(10,1), new(10,1), new(10,1), new(10,0)},
        {new(2,0),  new(2,0),  new(11,1), new(2,0),  new(2,0)},
        {new(2,0),  new(2,0),  new(12,1), new(2,0),  new(2,0)},
        {new(2,0),  new(2,0),  new(12,1), new(2,0),  new(2,0)}
    };
    */
}