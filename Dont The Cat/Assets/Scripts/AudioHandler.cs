using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clip
{
    Cat_Walking_On_Wood,
    Paper_Rustle,
    Agressive_Hissing,
    Aggressive_Meow = 3, //6 times
    Aggressive_Scream = 9, //3 times
    Aggressive_Pur,
    Calm_Meow = 11, //8 times (2 times deep voice)
    Calm_More_Pets_Please = 19,
    Calm_Pur,
    Cat_Exploding = 21, //2 times
    Computer_Beep_Boop = 23,
    Nuke_Boom = 24, //2 times
    Gun_Shot = 25 //2 times
}

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance;
    
    [Header("SOUND DESIGNER")]
    public AudioClip[] clips;
    private AudioSource source;

    void Awake()
    {
        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
    }

    private int getIndexForClipArray(Clip clip)
    {
        switch(clip)
        {
            case Clip.Aggressive_Meow: //6 times
                return (int)clip + Random.Range(0, 5);
            case Clip.Aggressive_Scream: //3 times
                return (int)clip + Random.Range(0, 2);
            case Clip.Calm_Meow: //8 times
                return (int)clip + Random.Range(0, 7);
            case Clip.Cat_Exploding: //2 times
                return (int)clip + Random.Range(0, 1);
            case Clip.Nuke_Boom: //2 times
                return (int)clip + Random.Range(0, 1);
            case Clip.Gun_Shot: //2 times
                return (int)clip + Random.Range(0, 1);
            default:
                return (int)clip;
        }
    }

    public void PlayLoopSound(Clip clip, float volume = 1f)
    {
        source.clip = clips[getIndexForClipArray(clip)];
        source.loop = true;
        source.volume = volume;

        if (source != null && !source.isPlaying)
        {
            source.Play();
        }
    }

    public void StopLoopSound()
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }

    public void PlaySingleSound(Clip clip, float volume = 1f)
    {
        AudioClip currentClip = clips[getIndexForClipArray(clip)];

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = currentClip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource, currentClip.length); // Zerst�rt das AudioSource-Objekt, nachdem der Clip abgespielt wurde
    }
}
