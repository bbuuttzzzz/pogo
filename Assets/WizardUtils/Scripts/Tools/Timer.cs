using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils.Tools
{
    public class Timer : MonoBehaviour
    {
        public UnityEvent OnTimerEnded;

        public bool CallOnAwake;

        public float Duration = 1;
        public float RandomRange = 0;

        public bool Realtime = false;


        public void Awake()
        {
            if (CallOnAwake) Call();
        }

        public void Call()
        {
            var duration = UnityEngine.Random.Range(Duration - RandomRange / 2, Duration + RandomRange / 2);
            StartCoroutine(RunTimer(duration));
        }

        IEnumerator RunTimer(float duration)
        {
            if (Realtime)
            {
                yield return new WaitForSecondsRealtime(duration);
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            EndTimer();
        }

        private void EndTimer()
        {
            OnTimerEnded?.Invoke();
        }
    }
}
