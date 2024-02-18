using System;
using System.Collections;
using System.Collections.Generic;

namespace WizardUtils
{
    public static class ArrayHelper
    {
        public static bool ContainsIndex<T>(T[] array, int index)
        {
            return index > 0 && index < array.Length;
        }

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

        /// <summary>
        /// Delete the member at <paramref name="deleteIndex"/>, reducing the array size by one<br/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="deleteIndex"></param>
        public static void DeleteAndResize<T>(ref T[] array, int deleteIndex)
        {
            T[] newArray = new T[array.Length - 1];
            int newIndex = 0;
            for (int oldIndex = 0; oldIndex < array.Length; oldIndex++)
            {
                if (oldIndex == deleteIndex)
                {
                    continue;
                }
                newArray[newIndex++] = array[oldIndex];
            }

            array = newArray;
        }


        public static void InsertAndResize<T>(ref T[] array, T newMember)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = newMember;

            array = newArray;
        }

        public static void InsertAndResize<T>(ref T[] array, T newMember, int insertIndex)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, 0, newArray, 0, insertIndex);
            Array.Copy(array, insertIndex, newArray, insertIndex + 1, array.Length - insertIndex);
            newArray[insertIndex] = newMember;

            array = newArray;
        }
    }
}