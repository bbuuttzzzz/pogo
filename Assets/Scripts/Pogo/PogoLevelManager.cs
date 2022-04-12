using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Pogo
{
    public class PogoLevelManager : MonoBehaviour
    {
        public bool LoadInitialLevelImmediately = true;

        void Start()
        {
            PogoGameManager game = GetComponent<PogoGameManager>();
            game.OnControlSceneChanged += onControlSceneChanged;
            if (LoadInitialLevelImmediately && !game.DontLoadScenesInEditor && game.InitialLevel != null)
            {
                LoadLevelInstantly(game.InitialLevel);
            }
        }

        private void onControlSceneChanged(object sender, ControlSceneEventArgs e)
        {
            if (e.InitialScene == null)
            {
                LoadDefaultAtmosphere();
            }
        }

        public LevelDescriptor CurrentLevel => currentLevel;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newLevel">the level to load</param>
        /// <param name="callback">called only if level loading starts successfully</param>
        /// <returns>FALSE if level is already loaded</returns>
        public bool LoadLevelAsync(LevelDescriptor newLevel, Action<LevelLoadingData> callback = null)
        {
            if (currentLevel == newLevel)
            {
                Debug.LogWarning($"Tried to load already-loaded level {newLevel}");
                return false;
            }
            currentLevel = newLevel;

            List<AsyncOperation> loadTasks = new List<AsyncOperation>();
            List<AsyncOperation> unloadTasks = new List<AsyncOperation>();
            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = getSceneDifference(newLevel);

            foreach (LevelDescriptor descriptor in scenesToLoad)
            {
                var task = SceneManager.LoadSceneAsync(descriptor.BuildIndex, LoadSceneMode.Additive);
                if (task != null)
                {
                    Debug.Log($"Loading scene {descriptor.name}");
                    loadTasks.Add(task);
                }
            }

            string sceneNames = "Already Loaded: ";
            foreach(Scene scene in scenesToUnload)
            {
                sceneNames += $"{scene.name} ";
                var task = SceneManager.UnloadSceneAsync(scene);
                Debug.Log($"Unloading scene {scene.name}");
                if (task != null) unloadTasks.Add(task);
            }
            Debug.Log(sceneNames);

            TransitionAtmosphere(newLevel, false);

            if (callback != null) callback(new LevelLoadingData(loadTasks, unloadTasks));
            return true;
        }

        internal void ResetLoadedLevel()
        {
            currentLevel = null;
        }

        #region Scenes

        static (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) getSceneDifference(LevelDescriptor newLevel)
        {
            List<LevelDescriptor> scenesToLoad = new List<LevelDescriptor>(newLevel.LoadLevels);
            List<Scene> scenesToUnload = new List<Scene>();

            // for each new scene
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (GameManager.ignoredScenes.Contains(scene.buildIndex)) continue;

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
        public GameObject DefaultPostProcessingPrefab;

        public void LoadDefaultAtmosphere()
        {
            TransitionAtmosphere(DefaultPostProcessingPrefab, true);
        }

        Atmosphere[] getExistingAtmospheres()
        {
            return AtmosphereParent.GetComponentsInChildren<Atmosphere>();
        }

        void TransitionAtmosphere(LevelDescriptor newLevel, bool instant)
        {
            TransitionAtmosphere(newLevel.PostProcessingPrefab, instant);
        }

        void TransitionAtmosphere(GameObject Prefab, bool instant)
        {
            // remove existing atmospheres
            var atmospheres = getExistingAtmospheres();
            foreach (Atmosphere atmosphere in atmospheres)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    DestroyImmediate(atmosphere.gameObject);
                }
                else
                {
                    atmosphere.DisableAndDestroy(instant);
                }
#else
                atmosphere.DisableAndDestroy(instant);
#endif
            }

            // add new atmosphere
            var newAtmosphereObj = Instantiate(Prefab, AtmosphereParent);
            var newAtmosphere = newAtmosphereObj.GetComponent<Atmosphere>();
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                newAtmosphere.SetWeightFromEditor(1);
            }
            else
            {
                newAtmosphere.SetWeight(1, instant);
            }
#else
            newAtmosphere.SetWeight(1, instant);
#endif
        }
        #endregion
    }
}
