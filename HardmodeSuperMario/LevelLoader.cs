using System;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using static MagicNumbers.Screen;

namespace Project
{
    internal class LevelLoader
    {
        delegate void SpriteCreator(int x, int y, string[] fields);

        private string path;
        private IGeometry[,] geometryGrid = new IGeometry[MaxLevelWidth, TileH];
        private Entity[,] entityGrid = new Entity[MaxLevelWidth, TileH];

        public LevelLoader(string level) =>
            path = "Content/Levels/" + level + ".csv";

        public LevelObjectManager LoadFromFile()
        {
            using TextFieldParser csvParser = new TextFieldParser(path);
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = false;

            while (!csvParser.EndOfData)
            {
                // Read current line fields, pointer moves to the next line.
                string[] fields = csvParser.ReadFields();
                typeof(LevelLoader).InvokeMember(
                    fields[0],
                    BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
                    null,
                    this,
                    new object[]
                    {
                        int.Parse(fields[1]),
                        int.Parse(fields[2]),
                        fields
                    }
                );
            }

            return new LevelObjectManager { Tiles = geometryGrid, Entities = entityGrid };
        }

        private void Background(int r, int g, string[] fields)
        {
            Game1.Instance.Background = new Color(
                r, g, int.Parse(fields[3])
            );
            switch (fields[4])
            {
                case "OverWorldThemeMusic":
                    Sound.BackgroundMusic = Sound.OverWorldThemeMusic;
                    break;
                case "UnderworldThemeMusic":
                    Sound.BackgroundMusic = Sound.UnderworldThemeMusic;
                    break;
            }
        }

        private void Mario(int x, int y, string[] _)
            => Project.Mario.Instance.Position = ISprite.Size * new Vector2(x, y);

        private void StopScroll(int x, int y, string[] _)
            => geometryGrid[x, y] = CommandBlock.StopScroll;

        private void Ground(int x, int y, string[] fields)
        {
            int right = x + int.Parse(fields[3]),
                bottom = y + int.Parse(fields[4]);

            for (int i = x; i < right; i++)
            {
                for (int j = y; j < bottom; j++)
                    geometryGrid[i, j] = Block.Ground;
            }
        }

        private void Ground2(int x, int y, string[] fields)
        {
            int right = x + int.Parse(fields[3]),
                bottom = y + int.Parse(fields[4]);

            for (int i = x; i < right; i++)
            {
                for (int j = y; j < bottom; j++)
                    geometryGrid[i, j] = Block.Ground2;
            }
        }

        private void BlueGround(int x, int y, string[] fields)
        {
            int right = x + int.Parse(fields[3]),
                bottom = y + int.Parse(fields[4]);

            for (int i = x; i < right; i++)
            {
                for (int j = y; j < bottom; j++)
                    geometryGrid[i, j] = Block.BlueGround;
            }
        }

        private void Hill(int x, int y, string[] fields)
        {
            for (int i = 0; i < Block.Hill.GetLength(0); i++)
            {
                for (int j = 0; j < Block.Hill.GetLength(1); j++)
                {
                    geometryGrid[x + i, y + j] = Block.Hill[i, j];
                }
            }
        }

        private void Bush(int x, int y, string[] fields)
        {
            int length = int.Parse(fields[3]);
            geometryGrid[x, y] = Block.Bush[0];
            for (int i = 0; i < length; i++)
            {
                geometryGrid[x + i + 1, y] = Block.Bush[1];
            }
            geometryGrid[x + length + 1, y] = Block.Bush[2];
        }

        private void Tree(int x, int y, string[] fields)
        {
            int height = int.Parse(fields[3]);
            geometryGrid[x, y] = Block.Tree[0];
            if (height == 2)
            {
                geometryGrid[x, y - 1] = Block.Tree[2];
                geometryGrid[x, y - 2] = Block.Tree[3];
            }
            else
                geometryGrid[x, y - 1] = Block.Tree[1];
        }

        private void Mushroom(int x, int y, string[] fields)
        {
            geometryGrid[x, y] = Block.Mushroom[0];
        }

        private void Cloud(int x, int y, string[] fields)
        {
            int length = int.Parse(fields[3]);
            geometryGrid[x, y] = Block.Cloud[0, 0];
            for (int i = 0; i < length; i++)
            {
                geometryGrid[x + i + 1, y] = Block.Cloud[0, 1];
            }
            geometryGrid[x + length + 1, y] = Block.Cloud[0, 2];
            geometryGrid[x, y + 1] = Block.Cloud[1, 0];
            for (int i = 0; i < length; i++)
            {
                geometryGrid[x + i + 1, y + 1] = Block.Cloud[1, 1];
            }
            geometryGrid[x + length + 1, y + 1] = Block.Cloud[1, 2];
        }
        private void SmileCloud(int x, int y, string[] fields)
        {
            int length = int.Parse(fields[3]);
            geometryGrid[x, y] = Block.SmileCloud[0, 0];
            for (int i = 0; i < length; i++)
            {
                geometryGrid[x + i + 1, y] = Block.SmileCloud[0, 1];
            }
            geometryGrid[x + length + 1, y] = Block.SmileCloud[0, 2];
            geometryGrid[x, y + 1] = Block.SmileCloud[1, 0];
            for (int i = 0; i < length; i++)
            {
                geometryGrid[x + i + 1, y + 1] = Block.SmileCloud[1, 1];
            }
            geometryGrid[x + length + 1, y + 1] = Block.SmileCloud[1, 2];
        }

        private void Brick(int x, int y, string[] fields)
        {
            if (fields[3].Length == 0)
                geometryGrid[x, y] = Block.Brick;
            else
            {
                var item = Enum.Parse<BlockWithItem.Item>(fields[3]);
                geometryGrid[x, y] = new BlockWithItem { Base = Block.Brick, Type = item };
            }
        }

        private void Brick2(int x, int y, string[] fields)
        {
            if (fields[3].Length == 0)
                geometryGrid[x, y] = Block.Brick2;
            else
            {
                var item = Enum.Parse<BlockWithItem.Item>(fields[3]);
                geometryGrid[x, y] = new BlockWithItem { Base = Block.Brick2, Type = item };
            }
        }

        private void BlueBrick(int x, int y, string[] fields)
            => geometryGrid[x, y] = Block.BlueBrick;

        private void HardBlock(int x, int y, string[] fields)
        {
            int height = int.Parse(fields[3]);
            for (int i = 0; i < height; i++)
            {
                geometryGrid[x, y + i] = Block.HardBlock;
            }
        }

        private void Coin(int x, int y, string[] fields)
        {
            geometryGrid[x, y] = Block.Coin;
        }

        private void QuestionMark(int x, int y, string[] fields)
        {
            var item = Enum.Parse<BlockWithItem.Item>(fields[3]);
            geometryGrid[x, y] = new BlockWithItem { Base = Block.QuestionMark, Type = item };
        }

        private void InvisibleBlock(int x, int y, string[] fields)
        {
            var item = BlockWithItem.Item.OneUp;
            Enum.TryParse<BlockWithItem.Item>(fields[3], out item);
            geometryGrid[x, y] = new InvisibleBlock { Type = item };
        }

        private void Stairs(int x, int y, string[] fields)
        {
            int size = int.Parse(fields[3]);
            bool northeast = bool.Parse(fields[4]);
            y -= size - 1;

            for (int i = 0; i < size; i++)
            {
                int offset = northeast ? x + i : x + size - i - 1;
                for (int j = size - 1; i + j >= size - 1; j--)
                    geometryGrid[offset, y + j] = Block.Stair;
            }
        }

        private void Pole(int x, int y, string[] fields)
        {
            geometryGrid[x, y] = Block.Pole[0];
            for (int i = 1; i < 9; i++)
            {
                geometryGrid[x, y + i] = Block.Pole[1];
            }
        }

        private void Pipe(int x, int y, string[] fields)
        {
            int height = int.Parse(fields[3]);
            geometryGrid[x, y] = Block.Pipe[0];
            geometryGrid[x + 1, y] = Block.Pipe[1];
            for (int i = 1; i < height; i++)
            {
                geometryGrid[x, y + i] = Block.Pipe[2];
                geometryGrid[x + 1, y + i] = Block.Pipe[3];
            }
        }

        private void WarpPipe(int x, int y, string[] fields)
        {
            Pipe(x, y, fields);

            int room = int.Parse(fields[4]);
            Vector2? position = null;
            if (float.TryParse(fields[5], out float posx))
                position = ISprite.Size * new Vector2(posx, int.Parse(fields[6]));
            geometryGrid[x, y] = new WarpPipe { Room = room, Position = position, Base = Block.Pipe[0] };
        }

        private void HorizontalPipe(int x, int y, string[] fields)
        {
            int height = int.Parse(fields[3]);
            geometryGrid[x, y] = Block.HorizontalPipe[0];
            geometryGrid[x, y + 1] = Block.HorizontalPipe[1];
            geometryGrid[x + 1, y] = Block.HorizontalPipe[2];
            geometryGrid[x + 1, y + 1] = Block.HorizontalPipe[3];
            geometryGrid[x + 2, y + 0] = Block.HorizontalPipe[4];
            geometryGrid[x + 2, y + 1] = Block.HorizontalPipe[5];
            for (int i = 1; i < height; i++)
            {
                geometryGrid[x + 2, y - i] = Block.HorizontalPipe[6];
            }
        }

        private void HorizontalWarpPipe(int x, int y, string[] fields)
        {
            HorizontalPipe(x, y, fields);

            int room = int.Parse(fields[4]);
            Vector2? position = null;
            if (float.TryParse(fields[5], out float posx))
                position = ISprite.Size * new Vector2(posx, int.Parse(fields[6]));
            geometryGrid[x, y + 1] = new WarpPipe { Horizontal = true, Room = room, Position = position, Base = Block.HorizontalPipe[1] };
        }

        private void SuperMushroom(int x, int y, string[] _) =>
            entityGrid[x, y] = new PowerUp(x, y, 0);

        private void Goomba(int x, int y, string[] _) =>
            entityGrid[x, y] = new Goomba(x, y);
        private void Koopa(int x, int y, string[] _) =>
            entityGrid[x, y] = new Koopa(x, y);
    }
}

