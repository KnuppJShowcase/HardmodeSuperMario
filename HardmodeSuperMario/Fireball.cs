using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project;

public class Fireball : Entity
{
    public static Fireball[] Instances { get; } = { new(), new() };

    private static readonly Texture2D Texture = TextureStorage.Instance.GetFireballSheet();
    private int CurrentFrame;
    private int TotalFrames = 4;
    private int Bounces;

    private bool MovingRight;

    //Assuming Mario is Fire Mario
    private Fireball() => Despawned = true;

    public override void Update()
    {
        CurrentFrame++;
        if (CurrentFrame == TotalFrames)
        {
            CurrentFrame = 0;
        }
        if (Position.Y > 230)
            Despawned = true;

        if (MovingRight)
        {
            Velocity = Velocity with { X = 3 };
        }
        else
        {
            Velocity = Velocity with { X = -3 };
        }
        // //Bounce if on ground
        // if (OnGround)
        // {
        //     Velocity = Velocity with { Y = -2.25f };
        // }
        base.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int width = Texture.Width / 2;
        int height = Texture.Height / 2;
        int row = CurrentFrame / 2;
        int column = CurrentFrame % 2;

        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        spriteBatch.Draw(Texture, Vector2.Floor(Position), sourceRectangle, Color.White);
    }

    //mario sprite position, mario direction
    public void ThrowAt(Vector2 vector, Direction direction)
    {
        Sound.Fireball.Play();
        Despawned = false;
        OnGround = false;
        Bounces = 0;
        Velocity = Velocity with { X = 0 };
        Velocity = Velocity with { Y = 0 };
        //If Mario facing Right
        if (direction == Direction.Right)
        {
            MovingRight = true;
            CurrentFrame = 0;
            Position = vector + new Vector2(16, -12);
        }
        else				//If Mario Facing Left
        {
            MovingRight = false;
            CurrentFrame = 1;
            Position = vector + new Vector2(-8, -12);
        }

    }

    static readonly Rectangle HitBoxSize = new Rectangle(-4, -4, 16, 16);
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
    public override void HitGround(Collision direction, Point _)
    {
        if (direction is Collision.Down && Bounces < 5)
        {
            Velocity = Velocity with { Y = -2.25f };
            Bounces++;
        }
        else
            Despawned = true;
    }
}
