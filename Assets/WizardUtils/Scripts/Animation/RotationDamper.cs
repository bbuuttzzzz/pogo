using UnityEngine;

namespace WizardUtils
{
    public class RotationDamper : MonoBehaviour
    {
        public Transform Constrained;
        public Transform Source;

        public float Damping;

        public bool UseWorldSpace;

        private void Update()
        {
            if (UseWorldSpace)
            {
                Constrained.rotation = InterpolateRotation(Constrained.rotation, Source.rotation, Time.deltaTime);
            }
            else
            {
                Constrained.localRotation = InterpolateRotation(Constrained.localRotation, Source.localRotation, Time.deltaTime);
            }
        }

        Quaternion InterpolateRotation(Quaternion start, Quaternion end, float deltaTime)
        {
            return Quaternion.Slerp(start, end, deltaTime * Damping);
        }
    }
}
