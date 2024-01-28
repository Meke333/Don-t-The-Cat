using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clip
{
    //walking lol
    Cat_Walking_On_Wood,
    //when cat arrives at interaction
    Paper_Rustle, 
    //when petted too much
    Agressive_Hissing = 2, //2 times
    //when petted too much
    Aggressive_Meow = 4, //6 times 
    Aggressive_Scream = 10, //3 times
    Aggressive_Pur = 13,
    //random?
    Calm_Meow = 14, //9 times (2 times deep voice)
    //when pettet too less
    Calm_Pet_Demanding = 23, //4 times
    //pleased cat
    Calm_Pur = 27, //2 times
    //at radio or self destruct
    Cat_Exploding = 29, //4 times 
    //clicking on a switch
    Computer_Beep_Boop = 33, //2 times
    //if you press nuke button?
    Button_Press = 35,
    //when you pressed nuke button and it was correct!
    Nuke_Boom = 36, //2 times  
    //when you get shot by cat
    Gun_Shot = 38, //2 times 
    //when cat hits selfdestruct button
    Selfdestruction_Siren = 40,
    Meow_Melody = 41,
    //if vase/urne fall down
    Glass_Shatter = 42, //2 times
    //when cat lands on ground/object
    Landing_Sound = 44
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
