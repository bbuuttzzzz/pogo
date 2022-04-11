using UnityEngine;

namespace WizardUtils
{
    public class RotationWaypointer : Waypointer<Vector3>
    {
        Quaternion baseOrientation;

        public override void Awake()
        {
            baseOrientation = transform.localRotation;
            base.Awake();
        }

        protected override Vector3 GetCurrentValue()
        {
            return (baseOrientation * Quaternion.Inverse(transform.localRotation)).eulerAngles;
        }

        protected override void InterpolateAndApply(Vector3 startValue, Vector3 endValue, float i)
        {
            Vector3 newEuler = Vector3.Lerp(startValue, endValue, i);

            transform.localRotation = baseOrientation * Quaternion.Euler(newEuler);
        }
    }
}
