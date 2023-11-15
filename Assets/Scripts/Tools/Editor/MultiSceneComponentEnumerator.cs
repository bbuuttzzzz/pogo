using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;

namespace Pogo.Tools
{
    public class MultiSceneComponentEnumerator<T> : IEnumerable<T>, IEnumerator<T> where T : MonoBehaviour
    {
        private string[] scenePaths;
        private int currentSceneIndex;
        private T[] currentSceneComponents;
        private int currentSceneComponentIndex;

        public string CurrentScenePath => scenePaths[currentSceneIndex];

        public MultiSceneComponentEnumerator() : this(GetAllScenes())
        {

        }

        public MultiSceneComponentEnumerator(string[] scenePaths)
        {
            this.scenePaths = scenePaths;
            Reset();
        }

        public T Current => currentSceneComponents[currentSceneComponentIndex];

        object IEnumerator.Current => Current;

        public void Reset()
        {
            LoadScene(0);
            currentSceneComponentIndex = 0;
        }

        public void Dispose()
        {
            LoadScene(0);
        }

        public bool MoveNext()
        {
            if (currentSceneComponentIndex >= currentSceneComponents.Length -1)
            {
                return LoadNextNonEmptyScene();
            }
            else
            {
                currentSceneComponentIndex++;
                return true;
            }
        }

        private bool LoadNextNonEmptyScene()
        {
            while(currentSceneIndex < scenePaths.Length - 1)
            {
                LoadScene(currentSceneIndex + 1);
                if (currentSceneComponents.Length > 0)
                {
                    currentSceneComponentIndex = 0;
                    return true;
                }
            }

            return false;
        }

        private void LoadScene(int sceneIndex)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                throw new InvalidOperationException("Aborted. User didn't want to save changes to a scene.");
            }

            currentSceneIndex = sceneIndex;
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePaths[sceneIndex], UnityEditor.SceneManagement.OpenSceneMode.Single);

            currentSceneComponents = UnityEngine.Object.FindObjectsOfType<T>();
        }

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        private static string[] GetAllScenes()
        {
            return EditorBuildSettings.scenes
                .Select(s => s.path)
                .ToArray();
        }
    }
}
