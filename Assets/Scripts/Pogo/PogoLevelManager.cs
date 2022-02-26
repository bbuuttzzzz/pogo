using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pogo
{
    public class PogoLevelManager : MonoBehaviour
    {
        void Start()
        {
            PogoGameManager game = GetComponent<PogoGameManager>();
            if (!game.dontLoadLevelsInEditor && game.InitialLevel != null)
            {
                LoadLevelInstantly(game.InitialLevel);
            }
        }

        LevelDescriptor currentLevel;
#if UNITY_EDITOR
        public void LoadLevelInEditor(LevelDescriptor newLevel)
        {
            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = getSceneDifference(newLevel);

            foreach (LevelDescriptor descriptor in scenesToLoad)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(descriptor.ScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }

            if ( UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                foreach (Scene scene in scenesToUnload)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
            }

            foreach(var atmosphere in getExistingAtmospheres())
            {
                DestroyImmediate(atmosphere.gameObject);
            }

            var newAtmosphereObj = UnityEditor.PrefabUtility.InstantiatePrefab(newLevel.PostProcessingPrefab, AtmosphereParent) as GameObject;
            var newAtmosphere = newAtmosphereObj.GetComponent<Atmosphere>();
            newAtmosphere.SetWeightFromEditor(1);
        }
#endif

        public void LoadLevelInstantly(LevelDescriptor newLevel)
        {
            if (currentLevel == newLevel)
            {
                Debug.LogWarning($"Tried to load already-loaded level {newLevel}");
                return;
            }
            currentLevel = newLevel;

            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = getSceneDifference(newLevel);

            foreach (LevelDescriptor descriptor in scenesToLoad)
            {
                SceneManager.LoadScene(descriptor.BuildIndex, LoadSceneMode.Additive);
            }

            foreach (Scene scene in scenesToUnload)
            {
                SceneManager.UnloadSceneAsync(scene);
            }

            TransitionAtmosphere(newLevel, true);
        }

        public void LoadLevelAsync(LevelDescriptor newLevel, Action<List<AsyncOperation>> callback = null)
        {
            if (currentLevel == newLevel)
            {
                Debug.LogWarning($"Tried to load already-loaded level {newLevel}");
                return;
            }
            currentLevel = newLevel;

            List<AsyncOperation> tasks = new List<AsyncOperation>();
            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = getSceneDifference(newLevel);

            foreach (LevelDescriptor descriptor in scenesToLoad)
            {
                var task = SceneManager.LoadSceneAsync(descriptor.BuildIndex, LoadSceneMode.Additive);
                tasks.Add(task);
            }

            foreach(Scene scene in scenesToUnload)
            {
                var task = SceneManager.UnloadSceneAsync(scene);
                tasks.Add(task);
            }

            TransitionAtmosphere(newLevel, false);

            if (callback != null) callback(tasks);
        }

        #region Scenes
        static readonly int[] ignoredScenes =
        {
            0 // this is GameScene
        };

        static (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) getSceneDifference(LevelDescriptor newLevel)
        {
            List<LevelDescriptor> scenesToLoad = new List<LevelDescriptor>(newLevel.LoadLevels);
            List<Scene> scenesToUnload = new List<Scene>();

            // for each new scene
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (ignoredScenes.Contains(scene.buildIndex)) continue;

                LevelDescriptor matchingLevel = null;

                // find the matching level if it exists
                foreach (LevelDescriptor sceneToLoad in scenesToLoad)
                {
                    if (sceneToLoad.BuildIndex == scene.buildIndex)
                    {
                        matchingLevel = sceneToLoad;
                    }
                }
                if (matchingLevel != null)
                {
                    // level already exists, so we don't need to load it
                    scenesToLoad.Remove(matchingLevel);
                }
                else
                {
                    // level no longer exists. so we need to get rid of it
                    scenesToUnload.Add(scene);
                }
            }

            return (scenesToLoad, scenesToUnload);
        }
        #endregion

        #region Atmosphere
        public Transform AtmosphereParent;

        Atmosphere[] getExistingAtmospheres()
        {
            return AtmosphereParent.GetComponentsInChildren<Atmosphere>();
        }

        void TransitionAtmosphere(LevelDescriptor newLevel, bool instant)
        {
            // remove existing atmospheres
            var atmospheres = getExistingAtmospheres();
            foreach(Atmosphere atmosphere in atmospheres)
            {
                atmosphere.DisableAndDestroy(instant);
            }

            // add new atmosphere
            var newAtmosphereObj = Instantiate(newLevel.PostProcessingPrefab, AtmosphereParent);
            var newAtmosphere = newAtmosphereObj.GetComponent<Atmosphere>();
            newAtmosphere.SetWeight(1, instant);
        }
        #endregion
    }
}
