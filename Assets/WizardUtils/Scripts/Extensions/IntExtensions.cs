using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// returns <paramref name="x"/> wrapped to the range [0, <paramref name="max"/>)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int PositiveModulo(this int x, int max)
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
        public static int Wrap(this int x, int min, int max)
        {
            return (x - min).PositiveModulo(max - min) + min;
        }
    }
}
