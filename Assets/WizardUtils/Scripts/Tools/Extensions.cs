
using UnityEngine;

namespace WizardUtils
{
    public static class RectExtensions
    {
        /// <summary>
        /// Splits the supplied rectangle into 2 rectangles in a row
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="splitFraction">a float from 0-1 of how far from the left to make the cut</param>
        /// <returns></returns>
        public static (Rect left, Rect right) CutRectHorizontally(Rect rect, float splitFraction = 0.5f)
        {
            Rect left = new Rect(rect.x, rect.y, rect.width * splitFraction, rect.height);
            Rect right = new Rect(rect.x + rect.width * splitFraction, rect.y, rect.width * (1 - splitFraction), rect.height);

            return (left, right);
        }

        public static Rect[] SplitRectHorizontally(Rect rect, params float[] partFractions)
        {
            Rect[] results = new Rect[partFractions.Length];
            float left = rect.x;
            for (int n = 0; n < partFractions.Length; n++)
            {
                float right = left + partFractions[n] * rect.width;
                results[n] = new Rect(left, rect.y, rect.width * partFractions[n], rect.height);

                left = right;
            }

            return results;
        }

        /// <summary>
        /// Splits the supplied rectangle into 2 rectangles in a column
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="splitFraction"></param>
        /// <returns>a float from 0-1 of how far from the top to make the cut</returns>
        public static (Rect top, Rect bottom) CutRectVertically(Rect rect, float splitFraction = 0.5f)
        {
            Rect top = new Rect(rect.x, rect.y, rect.width, rect.height * splitFraction);
            Rect bottom = new Rect(rect.x, rect.y + rect.height * splitFraction, rect.width, rect.height * (1 - splitFraction));

            return (top, bottom);
        }
    }
}
