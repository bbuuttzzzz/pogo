
using UnityEngine;

namespace WizardUI
{
    public static class Extensions
    {
        /// <summary>
        /// Splits the supplied rectangle into 2 rectangles in a row
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="splitFraction">a float from 0-1 of how far from the left to make the cut</param>
        /// <returns></returns>
        public static (Rect left, Rect right) SplitRectHorizontally(Rect rect, float splitFraction = 0.5f)
        {
            Rect left = new Rect(rect.x, rect.y, rect.width * splitFraction, rect.height);
            Rect right = new Rect(rect.x + rect.width * splitFraction, rect.y, rect.width * (1 - splitFraction), rect.height);

            return (left, right);
        }

        /// <summary>
        /// Splits the supplied rectangle into 2 rectangles in a column
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="splitFraction"></param>
        /// <returns>a float from 0-1 of how far from the top to make the cut</returns>
        public static (Rect top, Rect bottom) SplitRectVertically(Rect rect, float splitFraction = 0.5f)
        {
            Rect top = new Rect(rect.x, rect.y, rect.width, rect.height * splitFraction);
            Rect bottom = new Rect(rect.x, rect.y + rect.height * splitFraction, rect.width, rect.height * (1 - splitFraction));

            return (top, bottom);
        }
    }
}
