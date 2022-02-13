using System;
using UnityEngine;

namespace WizardUtils.Math
{
    public struct CoordinateSystem2D
    {
        Vector3 X;
        Vector3 Y;

        public CoordinateSystem2D(Vector3 x, Vector3 y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2 Project(Vector3 point)
        {
            return new Vector2(
                Vector3.Dot(X, point),
                Vector3.Dot(Y, point)
            );
        }

        internal Vector3 Compose(Vector2 point)
        {
            return point.x * X + point.y * Y;
        }
    }
}
