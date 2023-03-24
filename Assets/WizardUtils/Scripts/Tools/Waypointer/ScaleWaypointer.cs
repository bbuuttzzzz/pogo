using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils
{
    public class ScaleWaypointer : Waypointer<Vector3>
    {
        public Transform target;
        protected override Vector3 GetCurrentValue()
        {
            return target.localScale;
        }

        protected override void InterpolateAndApply(Vector3 startValue, Vector3 endValue, float i)
        {
            Vector3 newVector = Vector3.Lerp(startValue, endValue, i);
            target.localScale = newVector;
        }
    }
}

