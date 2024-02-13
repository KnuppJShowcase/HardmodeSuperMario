using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;
public class PowerUp : Entity
{
    private int XCoord, YCoord;
    private static readonly Texture2D Texture = TextureStorage.Instance.GetPowerUpSheet();
    public enum Sprite
    {
        SuperMushroom,  //0
        OneUpMushroom,  //1
        FireFlower,     //2
        SuperStar,      //3
        PoisonMushroom, //4
    }

    public Sprite SpriteName;
    private bool MovingRight = true;

    private enum State
    {
        Spawning,
        Spawned
    }

    private State state = State.Spawning;

    private bool animateFlower;

    //don't know exactly what needs to be done here, will need fixed when updating game1.cs 
    public PowerUp(int x, int y, Sprite spriteWanted) : base(x, y)
    {
        SpriteName = spriteWanted;
        //locations of SuperMushroom, OneUpMushroom, FireFlower, SuperStar, and Coin in the SpriteSheet
        XCoord = 0;
        YCoord = (int)spriteWanted * 16;
        if (spriteWanted == Sprite.OneUpMushroom)
        {
            XCoord = 16;
            YCoord = 0;
        } 
        if (spriteWanted == Sprite.PoisonMushroom)
        {
            XCoord = 32;
            YCoord = 0; 
        }
    }

    public override void Update()
    {
        if (state == State.Spawning)
        {
            Position -= new Vector2(0, 1);
            return;
        }

        //FireFlower and StationaryCoin are non-Moving
        if (SpriteName == Sprite.FireFlower)
        {
            if (animateFlower)
                XCoord = (XCoord + ISprite.Size) % 64;
            animateFlower = !animateFlower;
        }
        else if (SpriteName == Sprite.PoisonMushroom)
        {
            if (Mario.Instance.Position.X < Position.X)
            {
                Velocity = Velocity with { X = -1 };
            }
            else
            {
                Velocity = Velocity with { X = 1 };
            }
        }
        else
        {
            if (MovingRight)
            {
                Velocity = Velocity with { X = 1 };
            }
            else
            {
                Velocity = Velocity with { X = -1 };
            }
        }

        //SuperStar bounces when OnGround
        if (SpriteName == Sprite.SuperStar)
            if (OnGround)
            {
                Velocity = Velocity with { Y = -2.25f };
            }
        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Rectangle sourceRectangle = new Rectangle(XCoord, YCoord, ISprite.Size, ISprite.Size);
        spriteBatch.Draw(
            texture: Texture,
            position: Vector2.Floor(Position),
            sourceRectangle,
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: 1,
            effects: SpriteEffects.None,
            layerDepth: state == State.Spawning ? 1 : 0
        );
    }

    static readonly Rectangle HitBoxSize = new Rectangle(0, 0, ISprite.Size, ISprite.Size);
    public override Rectangle BoundingBox
    {
        get
        {
            var hitbox = HitBoxSize;
            hitbox.Offset(Position);
            return hitbox;
        }
    }
    public override Rectangle Hitbox => BoundingBox;
    public override void HitGround(Collision direction, Point size)
    {
        if (state == State.Spawning && size.Y == 1)
            state = State.Spawned;

        if (state == State.Spawned)
        {
            base.HitGround(direction, size);
            if (direction is Collision.Left or Collision.Right)
            {
                MovingRight = !MovingRight;
            }
        }
    }

    public void Collected(Mario mario)
    {
        switch (SpriteName)
        {
            case Sprite.SuperMushroom:
                mario.GetPowerUp(Mario.Power.Super);
                break;
            case Sprite.FireFlower:
                mario.GetPowerUp(Mario.Power.Fire);
                break;
            case Sprite.SuperStar:
                mario.GetStar();
                break;
            case Sprite.OneUpMushroom:
                Sound.OneUp.Play();
                Mario.Lives++;
                break;
            case Sprite.PoisonMushroom:
                mario.TakeDamage();
                break;
        }

        HUD.Instance.CollectPowerUp();
        Despawned = true;
    }
}
