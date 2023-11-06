using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Pogo.Tools
{
    public class AssetEnumerator<T> : IEnumerable<T>, IEnumerator<T> where T : UnityEngine.Object
    {
        private int currentIndex;
        private T[] array;

        public AssetEnumerator()
        {
            array = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                        .Select(id => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(id)))
                        .ToArray();
            Reset();
        }

        public T Current => array[currentIndex];

        object IEnumerator.Current => Current;

        public void Reset()
        {
            currentIndex = 0;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (currentIndex >= array.Length - 1)
            {
                return false;
            }
            else
            {
                currentIndex++;
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;
    }

}
