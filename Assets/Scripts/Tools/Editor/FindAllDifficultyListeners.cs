using Pogo;
using Pogo.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Tools.Editor
{
    public static class FindAllDifficultyListeners
    {
        [MenuItem("Pogo/Find All DifficultyListneers")]
        public static void Run()
        {
            foreach(var item in new MultiSceneComponentEnumerator<DifficultyListener>())
            {
                string path = AnimationUtility.CalculateTransformPath(item.transform, null);
                Debug.Log($"{EditorSceneManager.GetActiveScene().name} | {path}", item);
            }
        }
    }
}
