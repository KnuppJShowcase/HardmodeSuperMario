using System;
using System.Collections.Generic;
using static MagicNumbers.Screen;

namespace Project;

public class ScreenScroll
{
    public int Offset { get; private set; }

    public bool Update()
    {
        // If Mario is past the right half of the screen, update the offset
        int newOffset = Math.Max(Offset, (int)Mario.Instance.Position.X - Width / 2);

        bool shouldLoad = newOffset / ISprite.Size > Offset / ISprite.Size;
        Offset = newOffset;
        return shouldLoad;
    }

    public void DespawnOffscreen(List<Entity> entities)
    {
        int leftLimit = Offset - OffscreenLimit * ISprite.Size,
            bottomLimit = Height + OffscreenLimit;
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            if (entity.Despawned
                || (entity.Position.X >= leftLimit
                && entity.Position.Y <= bottomLimit))
                continue;

            if (entity is Mario mario)
                mario.Die(false);
            else
                entity.Despawned = true;
        }
    }
}
