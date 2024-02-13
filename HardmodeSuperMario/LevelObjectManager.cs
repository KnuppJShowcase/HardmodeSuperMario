using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static MagicNumbers.Screen;

namespace Project;

public class LevelObjectManager
{
    private static CollisionHandler collisionHandler = new();

    private List<Entity> loadedEntities = new() {
        Mario.Instance,
        Fireball.Instances[0],
        Fireball.Instances[1],
    };
    private ScreenScroll screenScroll { get; } = new();

    public bool MoveScreen { get; set; } = true;
    public Matrix ScreenOffset
        => Matrix.CreateTranslation(-screenScroll.Offset, 0, 0);

    public Entity MarioDecorator
    {
        get => loadedEntities[0];
        set => loadedEntities[0] = value;
    }

    public IGeometry[,] Tiles { private get; init; }
    public Entity[,] Entities { private get; init; }

    public void Load(Entity e)
    {
        for (int i = 3; i < loadedEntities.Count; i++)
        {
            if (loadedEntities[i].Despawned)
            {
                loadedEntities[i] = e;
                return;
            }
        }

        loadedEntities.Add(e);
    }

    public void LoadColumn(int x)
    {
        for (int y = 0; y < TileH; y++)
        {
            if (Entities[x, y] != null)
                Load(Entities[x, y]);
        }
    }

    static LevelObjectManager()
    {
        collisionHandler.RegisterCollision<PowerUp, Mario>("Collected");
        collisionHandler.RegisterCollision<Goomba, Mario>("Stomp");
        collisionHandler.RegisterCollision<Goomba, Fireball>("FireballHit");
        collisionHandler.RegisterCollision<Goomba, PowerUp>("PowerupHit");
        collisionHandler.RegisterCollision<Koopa, Mario>("Stomp");
        collisionHandler.RegisterCollision<Koopa, Fireball>("FireballHit");
        collisionHandler.RegisterCollision<Koopa, PowerUp>("PowerupHit");
    }

    public void ReloadScreen()
    {
        int xOffset = screenScroll.Offset / ISprite.Size;
        for (int x = xOffset; x <= xOffset + TileW + OffscreenLimit; x++)
            LoadColumn(x);
    }


    public void Update()
    {
        for (int i = 0; i < loadedEntities.Count; i++)
        {
            var entity = loadedEntities[i];
            if (!entity.Despawned) entity.Update();
        }
        // Prevent Mario from going past the left edge of the screen
        MarioDecorator.Position = Vector2.Max(MarioDecorator.Position, new Vector2(screenScroll.Offset, Single.NegativeInfinity));

        if (MoveScreen && screenScroll.Update())
            LoadColumn(
                screenScroll.Offset / ISprite.Size
                    + TileW
                    + OffscreenLimit
            );
        screenScroll.DespawnOffscreen(loadedEntities);

        int xOffset = screenScroll.Offset / ISprite.Size;
        for (int x = xOffset; x <= xOffset + TileW; x++)
        {
            for (int y = 0; y < 14; y++)
                Tiles[x, y] = Tiles[x, y]?.Update();
        }

        collisionHandler.DoCollisionCheck(loadedEntities);
        collisionHandler.DoGeometryCheck(loadedEntities, Tiles);
        // foreach (var block in levelLoader.LoadedGeometry)
        //     block.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var e in loadedEntities)
        {
            if (!e.Despawned) e.Draw(spriteBatch);
        }

        int xOffset = screenScroll.Offset / ISprite.Size;
        for (int x = xOffset; x <= xOffset + TileW; x++)
        {
            for (int y = 0; y < TileH; y++)
            {
                Tiles[x, y]?.Draw(spriteBatch, ISprite.Size * new Vector2(x, y));
            }
        }
    }
}
