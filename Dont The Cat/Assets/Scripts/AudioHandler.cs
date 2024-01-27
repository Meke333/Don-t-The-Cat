using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Clip
{
    Cricket,
    Bounce,
    Fall,
    Roll
}

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance;
    
    public AudioClip[] clips;
    private AudioSource source;

    void Awake()
    {
        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(Clip clip, bool loop)
    {
        source.clip = clips[(int)clip];
        source.loop = loop;

        if (source != null && !source.isPlaying)
        {
            source.Play();
        }
    }

    public void StopSound()
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }
}
