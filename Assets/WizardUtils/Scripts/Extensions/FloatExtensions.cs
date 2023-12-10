using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        /// returns <paramref name="x"/> wrapped to the range [0, <paramref name="max"/>)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float PositiveModulo(this float x, float max)
        {
            return (x % max + max) % max;
        }

        /// <summary>
        /// returns <paramref name="x"/> wrapped to the range [<paramref name="min"/>, <paramref name="max"/>)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Wrap(this float x, float min, float max)
        {
            return (x - min).PositiveModulo(max - min) + min;
        }
    }
}
