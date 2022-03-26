using System.Collections;
using System.Collections.Generic;

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
}