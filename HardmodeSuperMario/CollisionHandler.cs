using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Project;

[Flags]
public enum Collision
{
    Default,
    Up,
    Down,
    Left = 4,
    Right = 8
}

public class CollisionHandler
{
    private Dictionary<(Type, Type), MethodInfo> collisionMappings = new();

    // Given two method 
    public void RegisterCollision<T, U>(string methodName)
    {
        Type typeA = typeof(T), typeB = typeof(U);
        MethodInfo methodObj = null;
        // Check that the method actually exists
        if (methodName != null &&
            (methodObj = typeA.GetMethod(methodName, new[] { typeB })) == null)
            throw new ArgumentException();

        collisionMappings.Add((typeA, typeB), methodObj);
    }

    public void DoCollisionCheck(List<Entity> entities)
    {
        for (int i = 0; i < entities.Count - 1; i++)
        {
            if (entities[i].Despawned) continue;
            for (int j = i + 1; j < entities.Count; j++)
            {
                if (entities[j].Despawned) continue;
                Entity entityA = entities[i], entityB = entities[j];
                Rectangle hitboxA = entityA.Hitbox, hitboxB = entityB.Hitbox, intersection = Rectangle.Intersect(hitboxA, hitboxB);
                if (intersection.IsEmpty) continue;

                Collision directionA = GetDirection(hitboxA, intersection),
                    directionB = GetDirection(hitboxB, intersection);
                Type classA = entityA.GetType(), classB = entityB.GetType();

                MethodInfo methodObj;
                // Check if entityA has collision handling for entityB, first in the specified direction,
                // then in the default direction.
                if (collisionMappings.TryGetValue((classA, classB), out methodObj))
                    methodObj?.Invoke(entityA, new object[] { entityB });
                else
                    entityA.DefaultCollision(directionA);

                // Same thing, but with the entities reversed.
                if (collisionMappings.TryGetValue((classB, classA), out methodObj))
                    methodObj?.Invoke(entityB, new object[] { entityA });
                else
                    entityB.DefaultCollision(directionB);
            }
        }
    }

    public void DoGeometryCheck(List<Entity> entities, IGeometry[,] blocks)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            if (entity.Despawned) continue;
            entity.ResetOnGround();
            Rectangle boundingBox = entity.BoundingBox;
            int left = Math.Max(0, boundingBox.Left / ISprite.Size),
                right = Math.Min(blocks.GetLength(0) - 1, (boundingBox.Right - 1) / ISprite.Size),
                top = Math.Max(0, boundingBox.Top / ISprite.Size),
                bottom = Math.Min(blocks.GetLength(1) - 1, (boundingBox.Bottom - 1) / ISprite.Size);

            for (int x = left; x <= right; x++)
            {
                for (int y = bottom; y >= top; y--)
                {
                    if (blocks[x, y] != null) // Does a block exist here
                    {
                        var intersection = Rectangle.Intersect(
                            boundingBox,
                            new Rectangle(
                                x * ISprite.Size,
                                y * ISprite.Size,
                                ISprite.Size,
                                ISprite.Size
                            )
                        );

                        var direction = GetDirection(boundingBox, intersection);
                        blocks[x, y] = blocks[x, y].CollideWith(entity, direction, intersection);
                    }
                }
            }
        }
    }

    public static Collision GetDirection(Rectangle a, Rectangle intersection)
        => intersection switch
        {
            { Height: <= 4 } when a.Y < intersection.Y => Collision.Down,
            { Width: <= 3 } when a.X < intersection.X => Collision.Right,
            { Width: <= 3 } => Collision.Left,
            { Height: <= 4 } => Collision.Up,
            _ => Collision.Default
        };
}
