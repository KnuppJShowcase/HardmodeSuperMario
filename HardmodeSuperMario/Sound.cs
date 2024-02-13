using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Project;
public static class Sound
{
    //All Sound Effects Needed
    public static SoundEffect OneUp { get; private set; } // GOOD
    public static SoundEffect BowserFire { get; private set; } // not needed for 1-1
    public static SoundEffect BowserFall { get; private set; } // not needed for 1-1
    public static SoundEffect BreakBlock { get; private set; } // GOOD
    public static SoundEffect Bump { get; private set; }
    public static SoundEffect Coin { get; private set; }
    public static SoundEffect Fireball { get; private set; } // GOOD
    public static SoundEffect Flagpole { get; private set; }
    public static SoundEffect JumpSmall { get; private set; } //GOOD
    public static SoundEffect JumpSuper { get; private set; } // Wasn't Working, suggest not using
    public static SoundEffect Kick { get; private set; }
    public static SoundEffect LevelClear { get; private set; }
    public static SoundEffect PowerUpCollected { get; private set; } //GOOD
    public static SoundEffect PowerUpSpawn { get; private set; } // GOOD
    public static SoundEffect Stomp { get; private set; } //For Goomba Stomp/Fireball Hit and Koopa Stomp Twice/Fireball Hit GOOD
    public static SoundEffect TimeWarning { get; private set; }
    public static SoundEffect Vine { get; private set; } // not needed for 1-1
    public static SoundEffect WorldClear { get; private set; } // not needed for 1-1
    public static SoundEffectInstance OverWorldThemeMusic { get; private set; }

    public static SoundEffectInstance UnderworldThemeMusic { get; private set; }
    public static SoundEffectInstance StarMusic { get; private set; }
    public static SoundEffectInstance MarioDie { get; private set; } //GOOD
    public static SoundEffectInstance Pause { get; private set; } //GOOD
    public static SoundEffectInstance GameOver { get; private set; }
    public static SoundEffectInstance Pipe { get; private set; } //Also, Take Damage Sound - GOOD, Pipe Sound not implemented 

    private static SoundEffectInstance backgroundMusic;
    public static SoundEffectInstance BackgroundMusic
    {
        get => backgroundMusic;
        set
        {
            // Stop whatever was previously playing and start current BGM
            if (value != backgroundMusic)
            {
                backgroundMusic.Stop();
                backgroundMusic = value;
            }
            backgroundMusic.Play();
        }
    }

    public static void LoadContent(ContentManager Content)
    {
        SoundEffect.MasterVolume = 0.2f;
        OneUp = Content.Load<SoundEffect>("1-Up");
        BowserFire = Content.Load<SoundEffect>("BoswerFire");
        BowserFall = Content.Load<SoundEffect>("BowserFall");
        BreakBlock = Content.Load<SoundEffect>("BreakBlock");
        Bump = Content.Load<SoundEffect>("Bump");
        Coin = Content.Load<SoundEffect>("Coin");
        Fireball = Content.Load<SoundEffect>("FireballSound");
        Flagpole = Content.Load<SoundEffect>("Flagpole");
        GameOver = Content.Load<SoundEffect>("GameOver").CreateInstance();
        JumpSmall = Content.Load<SoundEffect>("JumpSmall");
        JumpSuper = Content.Load<SoundEffect>("JumpSuper");
        Kick = Content.Load<SoundEffect>("Kick");
        LevelClear = Content.Load<SoundEffect>("LevelClear");
        PowerUpCollected = Content.Load<SoundEffect>("PowerUpCollected");
        PowerUpSpawn = Content.Load<SoundEffect>("PowerUpSpawn");
        Stomp = Content.Load<SoundEffect>("Stomp");
        TimeWarning = Content.Load<SoundEffect>("TimeWarning");
        Vine = Content.Load<SoundEffect>("Vine");
        WorldClear = Content.Load<SoundEffect>("WorldClear");
        OverWorldThemeMusic = Content.Load<SoundEffect>("OverworldThemeMusic").CreateInstance();
        OverWorldThemeMusic.IsLooped = true;
        UnderworldThemeMusic = Content.Load<SoundEffect>("UnderworldThemeMusic").CreateInstance();
        UnderworldThemeMusic.IsLooped = true;
        StarMusic = Content.Load<SoundEffect>("StarEffectSound").CreateInstance();
        StarMusic.IsLooped = true;
        MarioDie = Content.Load<SoundEffect>("MarioDie").CreateInstance();
        Pause = Content.Load<SoundEffect>("Pause").CreateInstance();
        Pipe = Content.Load<SoundEffect>("Pipe").CreateInstance();
        backgroundMusic = OverWorldThemeMusic;
    }
}