using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static MagicNumbers.Koopa;

namespace Project;

public class Koopa : Entity
{
    private static readonly Texture2D Texture = TextureStorage.Instance.GetEnemySheet();
    private int frameCounter = 0;
    private int invincibilityFrames = 0;

    public bool upsideDown;

    private enum State
    {
        Flying,
        Walking,
        InShell,
        Kicked,
        Dead
    }

    private State state;

    private int superMushroomTimer = 0;
    private int superStarTimer = 0;

    public Koopa(int x, int y, bool flying = false) : base(x, y)
    {
        state = flying ? State.Flying : State.Walking;
        Velocity = new(-1, 0);
    }

    public override void Update()
    {
        if (FrameRule.IsZero)
        {
            if (invincibilityFrames > 0) invincibilityFrames--;
            if (superMushroomTimer > 0 && --superMushroomTimer == 0) Velocity /= new Vector2(3, 1);
            if (superStarTimer > 0) superStarTimer--;
        }
        frameCounter = (frameCounter + 1) % FrameTwo;
        // Apply limited antigravity to flying koopas
        if (state == State.Flying) Velocity -= new Vector2(0, 0.25f);

        // On hard mode, enemies follow you once close enough
        if (Game1.HardMode && state <= State.Walking
            && MathF.Abs(Mario.Instance.Position.X - Position.X)
                // Lower bound imposes a bit of a buffer so it isn't hovering over Mario
                is >= 2 * ISprite.Size and <= 5 * ISprite.Size)
        {
            DefaultCollision(Mario.Instance.Position.X > Position.X ? Collision.Left : Collision.Right);
        }

        base.Update();
    }

    public void Stomp(Mario mario)
    {
        switch (mario.Status)
        {
            case Mario.State.Default:
                bool marioTakesDamage = true;
                if (superStarTimer > 0) { }
                else if (state == State.InShell)
                {
                    Kick(mario.Position.X < Position.X);
                    marioTakesDamage = false;
                }
                else if (mario.Velocity.Y > 0)
                {
                    Shell();
                    marioTakesDamage = false;
                }

                if (marioTakesDamage && invincibilityFrames == 0)
                    mario.TakeDamage();
                else
                {
                    if (mario.Velocity.Y > 0)
                        mario.SetJumpVelocity();
                    Sound.Stomp.Play();
                    HUD.Instance.KillEnemy();
                    invincibilityFrames = 4;
                }
                break;
            case Mario.State.HasStar:
                Bounce(mario.Position.X < Position.X);
                state = State.Dead;
                break;
        }
    }

    // TODO: Add fireball animation
    public void FireballHit(Fireball fireball)
    {
        if (superStarTimer == 0)
        {
            Sound.Stomp.Play();
            Bounce(fireball.Position.X < Position.X);
            state = State.Dead;
        }
    }

    public void PowerupHit(PowerUp powerup)
    {
        if (!Game1.HardMode || state > State.Walking) return;

        if (powerup.SpriteName == PowerUp.Sprite.SuperMushroom)
        {
            Sound.PowerUpCollected.Play();
            superMushroomTimer = 14;
            Velocity *= new Vector2(3, 0);
            powerup.Despawned = true;
        }
        else if (powerup.SpriteName == PowerUp.Sprite.SuperStar)
        {
            Sound.PowerUpCollected.Play();
            superStarTimer = 14;
            powerup.Despawned = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (state <= State.Walking)
        {
            spriteBatch.Draw(
                texture: Texture,
                position: Vector2.Floor(Position),
                sourceRectangle: new Rectangle(TextureColumn * ISprite.Size, TextureRow * 2 * ISprite.Size, ISprite.Size, 2 * ISprite.Size),
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(0, ISprite.Size),
                scale: 1f,
                effects: Velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0f
            );
        }
        else
        {
            spriteBatch.Draw(
                texture: Texture,
                position: Vector2.Floor(Position),
                sourceRectangle: new Rectangle(TextureColumn * ISprite.Size, ISprite.Size, ISprite.Size, ISprite.Size),
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: upsideDown ? SpriteEffects.FlipVertically : SpriteEffects.None,
                layerDepth: 0f
            );
        }
    }

    readonly Rectangle hitBoxSize = new Rectangle(0, -8, ISprite.Size, 24);
    public override Rectangle BoundingBox
    {
        get
        {
            if (state == State.Dead) return Rectangle.Empty;
            var hitBox = hitBoxSize;
            hitBox.Offset(Position);
            return hitBox;
        }
    }

    public override Rectangle Hitbox => BoundingBox;

    public override void HitGround(Collision direction, Point size)
    {
        base.HitGround(direction, size);
        if (direction == Collision.Down)
        {
            if (state == State.Flying)
                Velocity = Velocity with { Y = -3 };
            else if (state == State.InShell)
                Velocity = Vector2.Zero;
        }
        else
            DefaultCollision(direction);
    }

    public override void DefaultCollision(Collision direction)
    {
        if (direction == Collision.Left)
        {
            Velocity = Velocity with { X = MathF.CopySign(Velocity.X, 1) };
        }
        else if (direction == Collision.Right)
        {
            Velocity = Velocity with { X = MathF.CopySign(Velocity.X, -1) };
        }
    }

    private int TextureColumn => state switch
    {
        State.Walking when frameCounter < FrameOne => 6,
        State.Walking => 7,
        State.Flying when frameCounter < FrameOne => 8,
        State.Flying => 9,
        _ => 10
    };

    private int TextureRow => superStarTimer > 0 ? frameCounter / 10 : 0;

    private void Shell()
    {
        // If we are going from walking to shell, kill all velocity.
        if (++state == State.InShell)
            Velocity = Vector2.Zero;
        // If we are going from flying to walking, only kill vertical velocity.
        else
            Velocity = Velocity with { Y = 0 };
    }

    private void Kick(bool right)
    {
        state = State.Kicked;
        Velocity = Velocity with { X = right ? 3 : -3 };
    }

    public override void Bounce(bool right)
    {
        state = State.InShell;
        upsideDown = true;
        base.Bounce(right);
    }
}
