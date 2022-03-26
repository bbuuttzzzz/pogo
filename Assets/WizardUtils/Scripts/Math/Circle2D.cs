using UnityEngine;

namespace WizardUtils.Math
{
    public struct Circle2D
    {
        public Vector2 Center;
        public float Radius;

        public Circle2D(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
