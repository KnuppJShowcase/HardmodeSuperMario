using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static MagicNumbers.HUD;

namespace Project
{
    public class HUD
    {
        private int coins = 0;
        public int time = StartingTime;
        private int score = 0;
        private SpriteFont font = TextureStorage.Instance.GetHUDFont();

        public static HUD Instance { get; } = new();
        private HUD() { }

        public void ResetTimer() => time = StartingTime;

        public void AddCoin()
        {
            coins++;
            Sound.Coin.Play();
            score += CoinScore;
            if (coins > CoinsToOneUp)
            {
                Sound.OneUp.Play();
                coins = 0;
            }
        }

        public void CollectPowerUp()
        {
            score += PowerUpScore;
        }

        public void BrickBreak()
        {
            Sound.BreakBlock.Play();
            score += BlockBreakScore;
        }

        public void KillEnemy()
        {
            score += EnemyScore;
        }

        public void Update()
        {
            if (FrameRule.IsZero)
            {
                time--;
                if (time == 100)
                {
                    Sound.TimeWarning.Play();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, "MARIO", new Vector2(MarioTitleX, MarioTitleY), Color.White);
            spriteBatch.DrawString(font, score.ToString(), new Vector2(ScoreX, ScoreY), Color.White);
            spriteBatch.DrawString(font, "COINS", new Vector2(CoinsX, 5), Color.White); // TODO: Magic number
            spriteBatch.DrawString(font, coins.ToString(), new Vector2(CoinsX, CoinsY), Color.White);
            spriteBatch.DrawString(font, "WORLD", new Vector2(WorldTitleX, WorldTitleY), Color.White);
            spriteBatch.DrawString(font, "1-1", new Vector2(WorldX, WorldY), Color.White);
            spriteBatch.DrawString(font, "TIME", new Vector2(TimeTitleX, TimeTitleY), Color.White);
            spriteBatch.DrawString(font, time.ToString(), new Vector2(TimeX, TimeY), Color.White);
        }
    }
}
