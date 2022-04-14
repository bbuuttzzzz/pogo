using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Math
{
    public struct Vector3Short
    {
        public short x, y, z;

        public static Vector3Short FromVector3Safe(Vector3 vec)
        {
            if (vec.x > short.MaxValue || vec.x < short.MinValue)
            {
                throw new ArgumentOutOfRangeException($"Could not cast float to short, value of x out of range");
            }
            if (vec.y > short.MaxValue || vec.y < short.MinValue)
            {
                throw new ArgumentOutOfRangeException($"Could not cast float to short, value of y out of range");
            }
            if (vec.z > short.MaxValue || vec.z < short.MinValue)
            {
                throw new ArgumentOutOfRangeException($"Could not cast float to short, value of z out of range");
            }
            return FromVector3(vec);
        }

        public static Vector3Short FromVector3(Vector3 vec)
        {
            return new Vector3Short()
            {
                // casting directly to short from float does nothing lol
                x = (short)Mathf.RoundToInt(vec.x),
                y = (short)Mathf.RoundToInt(vec.y),
                z = (short)Mathf.RoundToInt(vec.z)
            };
        }
    }

    public static class Vector3ShortExtensions
    {
        public static Vector3 ToVector3(this Vector3Short vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }
    }
}
