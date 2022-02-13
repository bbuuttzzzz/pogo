using UnityEngine;

namespace WizardUtils.Math
{
    /// <summary>
    /// A line stored in the form Ax + By + C = 0
    /// </summary>
    public struct StandardLine2D
    {
        public float A;
        public float B;
        public float C;

        public StandardLine2D(Vector2 point1, Vector2 point2)
        {
            if (point2.x == point1.x)
            {
                A = 1;
                B = 0;
                C = point1.x;
            }
            else
            {
                A = (point2.y - point1.y) / (point2.x - point1.x);
                B = -1;
                C = point1.y - A * point1.x;
            }
        }
    }
}
