using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clip
{
    Cat_Walking_On_Wood,
    Paper_Rustle,
    Nuke_Boom,
    Cat_Boom,
    Gun_Loading_Double_Peng,
    Gun_Loading_Peng
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

    public void PlayLoopSound(Clip clip, float volume = 1f)
    {
        source.clip = clips[(int)clip];
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
        AudioClip currentClip = clips[(int)clip];

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = currentClip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource, currentClip.length); // Zerst�rt das AudioSource-Objekt, nachdem der Clip abgespielt wurde
    }
}
