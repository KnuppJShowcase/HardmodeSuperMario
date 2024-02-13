using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class Goomba : Entity
{
    private static readonly Texture2D Texture = TextureStorage.Instance.GetEnemySheet();
    public int Rows { get; set; }
    public int Columns { get; set; }
    private int currentFrame;
    public int[] goombaWalk;
    public int[] goombaDie;
    public bool isDead;
    public int frameCounter;
    public int frameCounterColor;
    public int[] movePrio;

    private bool movingRight;
    public bool fireHit;
    public int fireCount;

    private int superMushroomTimer = 0;
    private int superStarTimer = 0;

    public Goomba(int x, int y) : base(x, y)
    {
        fireCount = 0;
        fireHit = false;
        Rows = 3;
        Columns = 3;
        currentFrame = 0;
        this.normalGoomba();
        isDead = false;
        movingRight = false;
    }

    public override void Update()
    {
        if (FrameRule.IsZero)
        {
            if (superMushroomTimer > 0) superMushroomTimer--;
            if (superStarTimer > 0) superStarTimer--;
        }

        if (fireHit == true)
        {
            fireDeath();
        }

        if (frameCounter < 20)
        {
            currentFrame = movePrio[0];
            frameCounter++;
        }
        else if (frameCounter < 40)
        {
            if (isDead)
            {
                Despawned = true;
                return;
            }
            currentFrame = movePrio[1];
            frameCounter++;
        }
        else
        {
            frameCounter = 0;
        }

        // On hard mode, enemies follow you once close enough
        if (Game1.HardMode && !isDead
            && MathF.Abs(Mario.Instance.Position.X - Position.X)
                // Lower bound imposes a bit of a buffer so it isn't hovering over Mario
                is >= 2 * ISprite.Size and <= 5 * ISprite.Size)
        {
            movingRight = Mario.Instance.Position.X > Position.X;
        }

        int speed = superMushroomTimer > 0 ? 3 : 1;
        if (movingRight && isDead == false)
        {
            Velocity = Velocity with { X = speed };
        }
        else if (!movingRight && isDead == false)
        {
            Velocity = Velocity with { X = -speed };
        }
        else
        {
            Velocity = Velocity with { X = 0 };
        }

        base.Update();
    }

    public void fireDeath()
    {
        if (fireCount < 5)
        {
            Velocity = Velocity with { X = 3 };
            Velocity = Velocity with { Y = -3 };
            fireCount++;
        }
        else if (fireCount < 10)
        {
            Velocity = Velocity with { X = 3 };
            Velocity = Velocity with { Y = 3 };
            fireCount++;
        }
        else
        {
            Despawned = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int width = ISprite.Size;
        int height = ISprite.Size;
        int row = superStarTimer > 0 ? 2 * (frameCounter / 10) + 1 : 1;
        int column = currentFrame % Columns;

        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);

        spriteBatch.Draw(Texture, Vector2.Floor(Position), sourceRectangle, Color.White);
    }

    public void Stomp(Mario mario)
    {
        switch (mario.Status)
        {
            case Mario.State.Default:
                if (superStarTimer == 0 && mario.Velocity.Y > 0)
                {
                    mario.SetJumpVelocity();
                    Sound.Stomp.Play();
                    isDead = true;
                    movePrio = goombaDie;
                    HUD.Instance.KillEnemy();
                }
                else
                    mario.TakeDamage();
                break;
            case Mario.State.HasStar:
                FireballHit(null);
                break;
        }
    }

    // TODO: Add fireball animation
    public void FireballHit(Fireball _)
    {
        if (superStarTimer == 0)
        {
            Sound.Stomp.Play();
            isDead = true;
            movePrio = goombaDie;
            fireHit = true;
        }
    }

    public void PowerupHit(PowerUp powerup)
    {
        if (!Game1.HardMode || isDead) return;

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

    public void normalGoomba()
    {
        isDead = false;
        movePrio = new int[2] { 0, 1 };
        goombaDie = new int[2] { 2, 2 };
    }

    public void blueGoomba()
    {
        isDead = false;
        movePrio = new int[2] { 3, 4 };
        goombaDie = new int[2] { 5, 5 };
    }

    public void whiteGoomba()
    {
        isDead = false;
        movePrio = new int[2] { 6, 7 };
        goombaDie = new int[2] { 8, 8 };
    }

    static readonly Rectangle TextureSize = new Rectangle(0, 0, ISprite.Size, ISprite.Size);
    static readonly Rectangle HitboxSize = new Rectangle(3, 6, 10, 6);
    public override Rectangle BoundingBox
    {
        get
        {
            var hitBox = TextureSize;
            hitBox.Offset(Position);
            return hitBox;
        }
    }

    public override Rectangle Hitbox
    {
        get
        {
            if (isDead) return Rectangle.Empty;
            if (fireHit) return Rectangle.Empty;
            var hitBox = HitboxSize;
            hitBox.Offset(Position);
            return hitBox;
        }
    }

    public override void HitGround(Collision direction, Point size)
    {
        base.HitGround(direction, size);
        DefaultCollision(direction);
    }

    public override void DefaultCollision(Collision direction)
    {
        if (direction == Collision.Left)
        {
            movingRight = true;
        }
        else if (direction == Collision.Right)
        {
            movingRight = false;
        }
    }
}

