using UnityEngine;

namespace WizardUtils
{
    public class AudioVolumeWaypointer : Waypointer<float>
    {
        public AudioSource Target;
        public bool PauseOnStop;

        protected override float GetCurrentValue()
        {
            return Target.volume;
        }

        protected override void InterpolateAndApply(float startValue, float endValue, float i)
        {
            float newValue = Mathf.Lerp(startValue, endValue, i);
            if (newValue > 0 && Target.volume == 0)
            {
                Target.Play();
            }
            else if (newValue == 0 && Target.volume > 0)
            {
                if (PauseOnStop)
                {
                    Target.Pause();
                }
                else
                {
                    Target.Stop();
                }
            }
            Target.volume = newValue;
        }
    }
}
