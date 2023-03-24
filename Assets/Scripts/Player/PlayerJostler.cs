using System.Collections;
using UnityEngine;

namespace Pogo
{
    public class PlayerJostler : MonoBehaviour
    {
        public float MaxJostleDistance;
        public AnimationCurve JostleStrengthCurve;
        public float Duration;
        public float Interval;

        public Coroutine JostleRoutine;

        public void Play()
        {
            if (JostleRoutine != null) Stop();
            JostleRoutine = StartCoroutine(Jostle());
        }

        public void Stop()
        {
            if (JostleRoutine == null) return;

            StopCoroutine(JostleRoutine);
            JostleRoutine = null;
            transform.localPosition = Vector3.zero;
        }

        float startTime;
        IEnumerator Jostle()
        {
            startTime = Time.unscaledTime;
            Vector3 normal = Camera.main.transform.forward;
            Vector3 direction = Vector3.Cross(normal, Vector3.up);
            Debug.DrawRay(transform.position, direction, Color.red,3);

            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(Interval);

            while (Time.unscaledTime < startTime + Duration)
            {
                float t = (Time.unscaledTime - startTime) / Duration;
                float maxDistance = MaxJostleDistance * JostleStrengthCurve.Evaluate(t);
                transform.localPosition = direction * maxDistance * (Random.value * 2 - 1);
                yield return wait;
            }
            transform.localPosition = Vector3.zero;
        }
    }
}
