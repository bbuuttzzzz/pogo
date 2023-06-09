using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    [RequireComponent(typeof(AudioSource))]
    public class SpeedWindEffect : MonoBehaviour
    {
        private AudioSource audioSource;
        public PlayerController Parent;

        public float AudioFadeStartSpeed = 10;
        public float AudioFadeEndSpeed = 20;
        public float MaxVolume = 1;

        private float RawCachedVolume = 0;
        public float VolumeMoveSpeed = 10;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            ResetVolume();
        }

        private void ResetVolume()
        {
            RawCachedVolume = 0;
            UpdateAudioSource();
        }

        private void LateUpdate()
        {
            float rawDesiredVolume;
            if (Parent.CurrentState == PlayerStates.Dead)
            {
                rawDesiredVolume = 0;
            }
            else
            {
                float speed = Parent.Velocity.magnitude;
                rawDesiredVolume = Mathf.InverseLerp(AudioFadeStartSpeed, AudioFadeEndSpeed, speed);
            }

            RawCachedVolume = Mathf.MoveTowards(RawCachedVolume, rawDesiredVolume, Time.unscaledDeltaTime * VolumeMoveSpeed);
            UpdateAudioSource();
        }

        private void UpdateAudioSource()
        {
            float finalVolume = RawToFinalVolume(RawCachedVolume);
            audioSource.volume = finalVolume;
            if (finalVolume > 0)
            {
                if (!audioSource.isPlaying) audioSource.Play();
            }
            else
            {
                if (audioSource.isPlaying) audioSource.Stop();
            }
        }
        float RawToFinalVolume(float rawVolume) => rawVolume * MaxVolume;
    }
}
