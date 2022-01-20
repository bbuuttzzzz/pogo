using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] Clips;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        int chosenSoundIndex = Random.RandomRange(0, Clips.Length);
        audioSource.PlayOneShot(Clips[chosenSoundIndex]);
    }
}
