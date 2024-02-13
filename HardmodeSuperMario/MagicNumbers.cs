namespace MagicNumbers
{
    public static class Screen
    {
        public const int MaxLevelWidth = 250;
        public const int TileW = 16;
        public const int TileH = 14;
        public const int Width = TileW * Project.ISprite.Size;
        public const int Height = TileH * Project.ISprite.Size;
        public const int SizeMult = 3;
        public const int ScaledWidth = Width * SizeMult;
        public const int ScaledHeight = Height * SizeMult;
        public const int OffscreenLimit = 4;
    }


    public static class HUD
    {
        public const int StartingTime = 499;
        public const int PowerUpScore = 1000;
        public const int CoinScore = 200;
        public const int CoinsToOneUp = 99;
        public const int BlockBreakScore = 50;
        public const int EnemyScore = 100;

        public const int MarioTitleX = 10;
        public const int MarioTitleY = 5;
        public const int ScoreX = 10;
        public const int ScoreY = 20;
        public const int CoinsX = 64;
        public const int CoinsY = 20;
        public const int WorldTitleX = 120;
        public const int WorldTitleY = 5;
        public const int WorldX = 120;
        public const int WorldY = 20;
        public const int TimeTitleX = 200;
        public const int TimeTitleY = 5;
        public const int TimeX = 200;
        public const int TimeY = 20;
    }

    public static class Entity
    {
        public const float GForce = 0.4f;
        public const float TerminalVelocity = 4;
    }

    public static class Mario
    {
        public const int StartLives = 3;

        // Number of frames in a single step of Mario's walking animation
        public const int TotalWalkFrames = 5;
        // Number of frames the star power lasts for
        public const int TotalStarFrames = 20;
        // Number of frames it takes for Mario to throw a fireball
        public const int TotalThrowFrames = 8;

        public const int TurningColumn = 3;
        public const int JumpingColumn = 4;
        public const int CrouchingColumn = 5;
        public const int StandingColumn = 6;
        public const int FireballColumn = 15;

        // Used to set Mario's velocity when he jumps
        public const float InitialJumpVelocity = -4.5f;
        public const float WalkingAcceleration = 0.1f;
        public const float Decceleration = 0.5f;
        public const float MaxXSpeed = 1.5f;
    }

    public static class Koopa
    {
        public const int FrameOne = 20;
        public const int FrameTwo = 40;
    }
}
