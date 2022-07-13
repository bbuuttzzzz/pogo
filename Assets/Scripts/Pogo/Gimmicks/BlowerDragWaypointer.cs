using UnityEngine;
using WizardUtils;

namespace Pogo
{
    public class BlowerDragWaypointer : Waypointer<float>
    {
        public Blower Target;

        protected override float GetCurrentValue()
        {
            return Target.Drag;
        }

        protected override void InterpolateAndApply(float startValue, float endValue, float i)
        {
            float newValue = Mathf.Lerp(startValue, endValue, i);
            Target.Drag = newValue;
        }
    }
}
