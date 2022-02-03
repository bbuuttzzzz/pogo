using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils
{
    public static class ArrayHelper
    {
        /// <summary>
        /// Delete the first instance of <paramref name="memberToDelete"/> if it exists, reducing the array size by one<br/>
        /// If it doesn't exist, the array will keep the same size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="memberToDelete"></param>
        /// <returns>true if the member was found and removed, else false</returns>
        public static bool DeleteAndResize<T>(ref T[] array, T memberToDelete)
        {
            List<T> list = new List<T>(array);
            if (list.Remove(memberToDelete))
            {
                array = list.ToArray();
                return true;
            }
            return false;
        }
    }

    static class Extensions
    {
        public static int BoolToAxis(bool positive, bool negative)
        {
            if (positive && !negative)
                return 1;
            if (negative && !positive)
                return -1;
            return 0;
        }

        /// <summary>
        /// plugs X into my sharpCurve function, slowly fades before suddenly spiking
        /// </summary>
        /// <param name="x">a value between 0 & 1</param>
        /// <returns>a value between 0 & 1</returns>
        public static float SharpCurve(float x)
        {
            if (x < .75f)
            {
                return 1 - 4 * x / 3;
            }
            else
            {
                return 4 * x - 3;
            }
        }

        /// <summary>
        /// remove the Y component from the vector3
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 Flatten(this Vector3 vec)
        {
            vec.y = 0;
            return vec;
        }

        /// <summary>
        /// Scales each component of a Vector3 by another vector3's components
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Vector3 Scale(this Vector3 vec, Vector3 other)
        {
            vec.x *= other.x;
            vec.y *= other.y;
            vec.z *= other.z;
            return vec;
        }

        /// <summary>
        /// Scale each component of a vector3 by respective x/y/z components
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Scale(this Vector3 vec, float x, float y, float z)
        {
            vec.x *= x;
            vec.y *= y;
            vec.z *= z;
            return vec;

        }
        /// <summary>
        /// gets the component of velocity along the slope of the surface
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="slopeNormal"></param>
        /// <returns>the new velocity to use</returns>
        public static Vector3 GetTangentComponent(this Vector3 vec, Vector3 slopeNormal)
        {
            return vec - vec.GetNormalComponent(slopeNormal);
        }
        /// <summary>
        /// gets the component of velocity pointing away from the surface
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3 GetNormalComponent(this Vector3 vec, Vector3 normal)
        {
            return Vector3.Dot(vec, normal) * normal;
        }
        /// <summary>
        /// returns a normalized vector along the plane tangent to upVector
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="upVector1"></param>
        /// <returns></returns>
        public static Vector3 Realign(this Vector3 vec, Vector3 upVector1)
        {
            return Vector3.Cross(upVector1, Vector3.Cross(vec.normalized, Vector3.up));
        }

        public static Vector3 Reciprocal(this Vector3 vec)
        {
            return new Vector3
            {
                x = 1 / vec.x,
                y = 1 / vec.y,
                z = 1 / vec.z
            };
        }

        public static Vector3Int MemberwiseRound(this Vector3 vec)
        {
            return new Vector3Int
            {
                x = Mathf.RoundToInt(vec.x),
                y = Mathf.RoundToInt(vec.y),
                z = Mathf.RoundToInt(vec.z)
            };
        }

        /// <summary>
        /// Clamps the magnitude of the vector to between the min and max size
        /// </summary>
        /// <param name="vec">a nonzero vector</param>
        /// <param name="min">minimum magnitude</param>
        /// <param name="max">maximum magnitude</param>
        /// <returns></returns>
        public static Vector3 Clamp(this Vector3 vec, float min, float max)
        {
            if (vec == Vector3.zero)
            {
                return Vector3.zero;
            }
            float magnitude = Mathf.Clamp(vec.magnitude, min, max);

            return vec.normalized * magnitude;

        }

        /// <summary>
        /// given a point p, and a triangle consisting of points a b and c, return the
        /// closest point to p on or within the triangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vector3 ClosestPointOnTri(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            // Uses algorithm from Ericson Real-Time collision detection sec. 5.1.5
            float w, v;

            // Check if P in vertex region outside A
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = p - a;
            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            if (d1 <= 0.0f && d2 <= 0.0f) return a; // barycentric coordinates (1,0,0)
                                                    // Check if P in vertex region outside B
            Vector3 bp = p - b;
            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);
            if (d3 >= 0.0f && d4 <= d3) return b; // barycentric coordinates (0,1,0)
                                                  // Check if P in edge region of AB, if so return projection of P onto AB
            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                v = d1 / (d1 - d3);
                return a + v * ab; // barycentric coordinates (1-v,v,0)
            }
            // Check if P in vertex region outside C
            Vector3 cp = p - c;
            float d5 = Vector3.Dot(ab, cp);
            float d6 = Vector3.Dot(ac, cp);
            if (d6 >= 0.0f && d5 <= d6) return c; // barycentric coordinates (0,0,1)

            // Check if P in edge region of AC, if so return projection of P onto AC
            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                w = d2 / (d2 - d6);
                return a + w * ac; // barycentric coordinates (1-w,0,w)
            }
            // Check if P in edge region of BC, if so return projection of P onto BC
            float va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                return b + w * (c - b); // barycentric coordinates (0,1-w,w)
            }
            // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
            float denom = 1.0f / (va + vb + vc);
            v = vb * denom;
            w = vc * denom;
            return a + ab * v + ac * w; // = u*a + v*b + w*c, u = va * denom = 1.0f-v-w
        }

        public static Vector2 ProjectVertical(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        /// <summary>
        /// Gives the angle between two Vector2s relative to (0,0), giving a negative answer if left is clockwise of right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static float AngleBetween(Vector2 left, Vector2 right)
        {
            float leftAngle = Mathf.Atan2(left.y, left.x) * Mathf.Rad2Deg;
            float rightAngle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;

            float baseBetween = leftAngle - rightAngle;
            if (Mathf.Abs(baseBetween) > 180)
            {
                return baseBetween - 360 * Mathf.Sign(baseBetween);
            }
            else
            {
                return baseBetween;
            }
        }

        public static Vector2 RotateRadians(this Vector2 vec, float radians)
        {
            return new Vector2(
              Mathf.Cos(radians) * vec.x - Mathf.Sin(radians) * vec.y,
              Mathf.Sin(radians) * vec.x + Mathf.Cos(radians) * vec.y
            );

        }

        // Interpolation Stuff
        /// <summary>
        /// Interpolate from a to b by parametric t with a smooth start and stop
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SmoothInterpolate(float a, float b, float t)
        {
            return a + (b - a) * t * t * (3 - 2 * t);
        }
        public static Color SmoothInterpolate(Color a, Color b, float t)
        {
            for (int n = 0; n < 4; n++)
            {
                a[n] = SmoothInterpolate(a[n], b[n], t);
            }

            return a;
        }
        public static Vector2 SmoothInterpolate(Vector2 a, Vector2 b, float t)
        {
            for (int n = 0; n < 2; n++)
            {
                a[n] = SmoothInterpolate(a[n], b[n], t);
            }

            return a;
        }

        /// <summary>
        /// Interpolate from a to b by parametric t with a smooth start
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float AccelerateInterpolate(float a, float b, float t)
        {
            return a + (b - a) * t * t;
        }
        public static Color AccelerateInterpolate(Color a, Color b, float t)
        {
            for (int n = 0; n < 4; n++)
            {
                a[n] = AccelerateInterpolate(a[n], b[n], t);
            }

            return a;
        }
        public static Vector2 AccelerateInterpolate(Vector2 a, Vector2 b, float t)
        {
            for (int n = 0; n < 2; n++)
            {
                a[n] = AccelerateInterpolate(a[n], b[n], t);
            }

            return a;
        }
    }

    public struct Tuple<T>
    {
        public T first;
        public T second;

        public Tuple(T first, T second)
        {
            this.first = first;
            this.second = second;
        }

        public Tuple<T> GetInverse()
        {
            return new Tuple<T>(second, first);
        }
        public static bool CheckInverses(Tuple<T> left, Tuple<T> right)
        {
            return left.first.Equals(right.second) && left.second.Equals(right.first);
        }
    }
}