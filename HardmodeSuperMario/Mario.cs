using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static MagicNumbers.Mario;

namespace Project;
public enum Direction
{
    Left = -1,
    Right = 1
}

public class Mario : Entity
{
    public static Mario Instance { get; private set; } = new();
    public static int Lives { get; set; }

    static readonly Rectangle SmallHitbox = new Rectangle(2, 3, 12, 13);
    static readonly Rectangle SuperHitbox = new Rectangle(2, -8, 12, 24);
    static readonly Rectangle CrouchingHitbox = new Rectangle(2, 4, 12, 12);

    private static readonly Texture2D Texture = TextureStorage.Instance.GetMarioSheet();

    public override Rectangle BoundingBox
    {
        get
        {
            if (WarpPipeVeclocity != null) return Rectangle.Empty;
            var hitbox = this switch
            {
                { Crouching: true } => CrouchingHitbox,
                { Powerup: > Power.Small } => SuperHitbox,
                _ => SmallHitbox
            };
            hitbox.Offset(Position);
            return hitbox;
        }
    }
    public override Rectangle Hitbox => BoundingBox;

    public enum State
    {
        Default,
        Flickering,
        HasStar
    }
    public State Status { get; private set; } = State.Default;
    public Vector2? WarpPipeVeclocity { get; set; } = null;
    private bool drawFlicker = true;
    private int invunerabilityTimer;
    private int starPaletteCycle = 0;

    public enum Power
    {
        Small,
        Super,
        Fire,
    }
    public Power Powerup { get; private set; } = Power.Small;

    [Flags]
    enum Movement
    {
        None,
        Walking,
        Jumping,
        Crouching = 4,
    }
    private Movement movement = Movement.None;
    private int fireballTimer = 0;
    private bool Walking => (movement & (Movement.Walking | Movement.Crouching)) == Movement.Walking;
    private bool Jumping => movement.HasFlag(Movement.Jumping);
    private bool Crouching => movement.HasFlag(Movement.Crouching);


    // When Mario's moving, keeps track of the texture to draw
    private int walkCycle = 0;
    private int walkTimer = TotalWalkFrames;

    private Direction facing = Direction.Right;
    public SpriteEffects Effects => facing == Direction.Left
        ? SpriteEffects.FlipHorizontally
        : SpriteEffects.None;

    private Mario() { }

    public override void Update()
    {
        if (WarpPipeVeclocity is Vector2 velocity)
        {
            Position += velocity;
            return;
        }

        CheckDebugKeys();
        CheckMovementKeys();
        ThrowFireball();
        Velocity = Velocity with { X = Math.Clamp(Velocity.X + HorizAccel, -MaxXSpeed, MaxXSpeed) };

        if (Status != State.Default)
        {
            if (FrameRule.IsZero && --invunerabilityTimer == 0)
                BecomeVulnerable();
            else
            {
                drawFlicker = !drawFlicker;
                if (drawFlicker && Status == State.HasStar)
                    starPaletteCycle = (starPaletteCycle + 1) % 3;
            }
        }

        if (fireballTimer > 0)
            fireballTimer--;
        if (OnGround && Walking)
        {
            if (walkTimer == 0)
            {
                walkCycle = (walkCycle + 1) % 3;
                walkTimer = TotalWalkFrames;
            }
            else
                walkTimer--;
        }

        if (HUD.Instance.time <= 0)
        {
            Instance.Die();
            HUD.Instance.ResetTimer();
        }

        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Status == State.Flickering && !drawFlicker)
            return;

        bool large = Powerup > Power.Small;

        Rectangle? sourceRectangle = new Rectangle(
            ISprite.Size * TextureColumn,
            ISprite.Size * TextureRow,
            ISprite.Size,
            large ? 2 * ISprite.Size : ISprite.Size
        );

        spriteBatch.Draw(
            Texture,
            Vector2.Floor(Position),
            sourceRectangle,
            Color.White,
            0f,
            // If Mario is super/fire, we want to shift the upper-left corner up by 16px
            large ? new Vector2(0, ISprite.Size) : Vector2.Zero,
            1f,
            // All of the Mario sprites are facing right, so flip the texture if he's facing left
            Effects,
            // Draw in background when entering warp pipe
            WarpPipeVeclocity == null ? 0 : 1
        );
    }

    public override void HitGround(Collision direction, Point size)
    {
        switch (direction)
        {
            case Collision.Down:
                movement &= ~Movement.Jumping;
                break;
            // Reset X velocity
            case Collision.Left:
            case Collision.Right:
                Velocity = Velocity with { X = 0 };
                break;
        }

        base.HitGround(direction, size);
    }

    public void SetJumpVelocity()
        => Velocity = Velocity with { Y = InitialJumpVelocity };

    public void GetPowerUp(Power power)
    {
        Sound.PowerUpCollected.Play();
        if (Powerup < power)
        {
            Powerup = power;
            // TODO: Add separate animation for super to fire
            var animation = MarioAnimation.SmallToSuper;
            animation.PlayAt(Position, Effects);
        }
    }

    public void GetStar()
    {
        Status = State.HasStar;
        Sound.BackgroundMusic = Sound.StarMusic;
        invunerabilityTimer = 36;
    }

    public void TakeDamage()
    {
        if (Powerup == Power.Small)
            Die();
        else
        {
            Sound.Pipe.Play(); //Pipe Sound is the same as take damage sound. 
            Powerup = Power.Small;
            Status = State.Flickering;
            invunerabilityTimer = 10;
            MarioAnimation.SuperToSmall.PlayAt(Position, Effects);
        }
    }

    public void BecomeVulnerable()
    {
        if (Status == State.HasStar)
            Sound.BackgroundMusic = Sound.OverWorldThemeMusic;
        Status = State.Default;
    }

    public void Die(bool onScreen = true)
    {
        // When we die, we need to reset everything about Mario's state
        // This is less code than resetting each field manually
        Instance = new();
        Lives--;
        DeadMario.Instance.PlayAt(Position, onScreen);
    }

    // Spawns a fireball at Mario's position if he has a fire flower
    public void ThrowFireball()
    {
        Fireball fireball;
        if (KeyboardController.IsThrowingFireball() && Powerup == Power.Fire && fireballTimer == 0
            && (fireball = Array.Find(Fireball.Instances, f => f.Despawned)) != null)
        {
            fireballTimer = TotalThrowFrames;
            fireball.ThrowAt(Position, facing);
        }
    }

    private void CheckDebugKeys()
    {
        if (KeyboardController.MakeMarioSuper()) GetPowerUp(Power.Super);
        else if (KeyboardController.MakeMarioFire()) GetPowerUp(Power.Fire);
        else if (KeyboardController.DamageMario()) TakeDamage();
        if (KeyboardController.MakeMarioStar()) GetStar();
    }

    private void CheckMovementKeys()
    {
        if (KeyboardController.IsMovingLeft())
        {
            facing = Direction.Left;
            movement |= Movement.Walking;
        }
        else if (KeyboardController.IsMovingRight())
        {
            facing = Direction.Right;
            movement |= Movement.Walking;
        }
        else
            movement &= ~Movement.Walking;

        if (Powerup == Power.Small || !KeyboardController.IsCrouching())
            movement &= ~Movement.Crouching;
        else if (OnGround)
            movement |= Movement.Crouching;

        if (OnGround && KeyboardController.IsJumping())
        {
            Sound.JumpSmall.Play();
            movement |= Movement.Jumping;
            SetJumpVelocity();
        }
        else if (Jumping && Velocity.Y < 0 && KeyboardController.IsHoldingJump())
            Velocity -= new Vector2(0, 0.25f);
    }

    // Used by Draw() to select the correct sprite from Mario's texture atlas.

    private int TextureColumn => this switch
    {
        { Crouching: true } => CrouchingColumn,
        { Powerup: Power.Fire, fireballTimer: > 0 } => FireballColumn,
        { Jumping: true } => JumpingColumn,
        { Velocity.X: 0, Walking: true } => walkCycle,
        { Velocity.X: 0 } => StandingColumn,
        _ when Math.Sign(Velocity.X) == (int)facing => walkCycle,
        _ => TurningColumn
    };

    // TODO: Change row when Star mario
    private int TextureRow
    {
        get
        {
            int row = 3 * this switch
            {
                { Status: State.HasStar, starPaletteCycle: int i } => i + 2,
                { Powerup: Power.Fire } => 1,
                _ => 0
            };
            return Powerup > Power.Small ? row : row + 2;
        }
    }
    
    //Troll the player by killing them when they touch the flagpole!
    //This is hardmode afterall
    public void grabFlagDie()
    {
        Die(true);
    }

    private float HorizAccel =>
        this switch
        {
            { EjectedFromWall: true } => 0,
            // If Mario is walking left/right, then we accelerate left/right
            { facing: Direction.Left, Walking: true } => -WalkingAcceleration,
            { facing: Direction.Right, Walking: true } => WalkingAcceleration,
            // Otherwise, if mario is on the ground, we want to deccelerate
            // (i.e., accelerate in the opposite direction of Mario's velocity)
            _ when OnGround => -1 * Math.Sign(Velocity.X) * Math.Min(Math.Abs(Velocity.X), Decceleration),
            _ => 0
        };
}
