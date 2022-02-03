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
            int chosenSoundIndex = Random.Range(0, Clips.Length);
            audioSource.PlayOneShot(Clips[chosenSoundIndex]);
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (canPlay())
        {
            lastPlay = Time.time;
            audioSource.PlayOneShot(clip);
        }
    }

    private bool canPlay()
    {
        return lastPlay + ReplayDelay < Time.time;
    }
}
