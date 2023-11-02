using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] Clips;
    /// <summary>
    /// How long until Play() will produce another sound
    /// </summary>
    public float ReplayDelay;

    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    float lastPlay = float.MinValue;
    public void Play()
    {
        if (canPlay() && Clips.Length > 0)
        {
            lastPlay = Time.time;
            audioSource.PlayOneShot(NextRandomSound());
        }
    }

    public void PlayOneShot(AudioClip clip) => PlayOneShot(clip, false);

    public void PlayOneShot(AudioClip clip, bool ignoreReplayDelay)
    {
        if (canPlay() || ignoreReplayDelay)
        {
            lastPlay = Time.time;
            audioSource.PlayOneShot(clip);
        }
    }

    private bool canPlay()
    {
        return lastPlay + ReplayDelay < Time.time;
    }

    int previousIndex = -1;
    private AudioClip NextRandomSound()
    {
        if (previousIndex >= 0 && previousIndex < Clips.Length)
        {
            int index = Random.Range(0, Clips.Length - 1);
            if (index >= previousIndex) index++;

            previousIndex = index;
            return Clips[index];
        }
        else
        {
            previousIndex = Random.Range(0, Clips.Length);
            return Clips[previousIndex];
        }
    }
}
