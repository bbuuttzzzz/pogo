using UnityEngine;
using WizardUtils;

namespace Pogo
{
    public class RotaterSpeedWaypointer : Waypointer<float>
    {
        public Rotater Target;

        protected override float GetCurrentValue()
        {
            return Target.RotationSpeed;
        }

        protected override void InterpolateAndApply(float startValue, float endValue, float i)
        {
            float newValue = Mathf.Lerp(startValue, endValue, i);
            Target.RotationSpeed = newValue;
        }
    }
}
