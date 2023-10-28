using Pogo.Levels.Loading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Pogo.Levels
{
    public class PogoLevelManager : MonoBehaviour
    {
        public bool LoadInitialLevelImmediately = true;
        public List<LevelSceneLoader> CurrentSceneLoaders;

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

#if UNITY_EDITOR
        public void SetCurrentLevelInEditor(LevelDescriptor level)
        {
            currentLevel = level;
        }
#endif
        public LevelDescriptor CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;
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

            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                foreach (Scene scene in scenesToUnload)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
            }

            foreach (var atmosphere in getExistingAtmospheres())
            {
                DestroyImmediate(atmosphere.gameObject);
            }

            var newAtmosphereObj = UnityEditor.PrefabUtility.InstantiatePrefab(newLevel.PostProcessingPrefab, AtmosphereParent) as GameObject;
            var newAtmosphere = newAtmosphereObj.GetComponent<Atmosphere>();
            newAtmosphere.SetWeightFromEditor(1);
            PogoGameManager.PogoInstance._CachedCheckpoint = null;
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
        /// <param name="newLevel">the Level to load</param>
        /// <param name="settings"></param>
        /// <returns>FALSE if Level is already loaded</returns>
        public bool LoadLevelAsync(LevelLoadingSettings settings)
        {
            if (currentLevel == settings.Level && !settings.ForceReload)
            {
                if (settings.LevelState.HasValue)
                {
                    PogoGameManager.PogoInstance.SetLevelState(settings.LevelState.Value, settings.Instantly);
                }
                return false;
            }
            currentLevel = settings.Level;

            List<AsyncOperation> loadTasks = new List<AsyncOperation>();
            List<AsyncOperation> unloadTasks = new List<AsyncOperation>();
            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = getSceneDifference(settings.Level);

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
            foreach (Scene scene in scenesToUnload)
            {
                sceneNames += $"{scene.name} ";
                var task = SceneManager.UnloadSceneAsync(scene);
                Debug.Log($"Unloading scene {scene.name}");
                if (task != null) unloadTasks.Add(task);
            }

            Action callBack = () =>
            {
                TransitionAtmosphere(CurrentLevel, settings.Instantly);
                PogoGameManager.PogoInstance.OnLevelLoaded?.Invoke();
                if (settings.LevelState.HasValue)
                {
                    PogoGameManager.PogoInstance.SetLevelState(settings.LevelState.Value, settings.Instantly);
                }

                foreach (var initialLevelState in settings.Level.LoadLevelStates)
                {
                    PogoGameManager.PogoInstance.TryInitializeLevelStateForLevel(initialLevelState, settings.Instantly);
                }
            };

            loadLevelScenesInOrder(new LevelLoadingData(loadTasks, unloadTasks), settings, callBack);

            return true;
        }

        internal void ResetLoadedLevel()
        {
            currentLevel = null;
        }


        IEnumerator loadLevelScenesSimultaneous(LevelLoadingData levelLoadingData, LevelLoadingSettings settings, Action callback = null)
        {
            foreach (AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = false;
            }

            bool finished = false;
            while (!finished)
            {
                float progress = 0;
                finished = true;

                string txt = "";
                foreach (AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
                {
                    progress += Task.isDone ? 1 : Task.progress / 0.9f;
                    finished = finished && (Task.progress >= 0.9f || Task.isDone);
                    txt = txt + Task.progress + " ";
                }

                progress /= levelLoadingData.LoadingSceneTasks.Count;
                Debug.Log($"Progress: %{progress * 100:N2} -- {txt}");

                yield return new WaitForSecondsRealtime(0.02f);
            }

            foreach (AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = true;
            }
            finished = false;
            while (!finished)
            {
                float progress = 0;
                finished = true;

                string txt = "";
                foreach (AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
                {
                    progress += Task.isDone ? 1 : Task.progress;
                    finished = finished && Task.isDone;
                    txt = txt + Task.progress + " ";
                }

                progress /= levelLoadingData.LoadingSceneTasks.Count;
                Debug.Log($"Progress: %{progress * 100:N2} -- {txt}");

                yield return new WaitForSecondsRealtime(0.02f);
            }

            if (settings.LoadingFromMenu)
            {
                PogoGameManager.PogoInstance.UnloadControlScene();
                PogoGameManager.PogoInstance.ResetStats(settings.QuickSaveData);
            }

            callback?.Invoke();
        }

        IEnumerator loadLevelScenesInOrder(LevelLoadingData levelLoadingData, LevelLoadingSettings settings, Action callback = null)
        {
            foreach (AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = false;
            }

            int completed = 0;
            foreach (AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
            {
                bool finished = false;
                while (!finished)
                {
                    finished = Task.progress >= 0.9f || Task.isDone;

                    Debug.Log($"Progress: %{Task.progress * 100:N2} ({completed + 1}/{levelLoadingData.LoadingSceneTasks.Count})");

                    if (finished)
                    {
                        completed++;
                        Task.allowSceneActivation = true;
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(0.02f);
                    }
                }
            }

            bool cleanupFinished = false;
            while (!cleanupFinished)
            {
                float progress = 0;
                cleanupFinished = true;

                string txt = "";
                foreach (AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
                {
                    progress += Task.isDone ? 1 : Task.progress;
                    cleanupFinished = cleanupFinished && Task.isDone;
                    txt = txt + Task.progress + " ";
                }

                progress /= levelLoadingData.LoadingSceneTasks.Count;
                Debug.Log($"Progress: %{progress * 100:N2} -- {txt}");


                if (cleanupFinished)
                {
                    break;
                }
                else
                {
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }

            if (settings.LoadingFromMenu)
            {
                PogoGameManager.PogoInstance.UnloadControlScene();
                PogoGameManager.PogoInstance.ResetStats(settings.QuickSaveData);
            }

            callback?.Invoke();
        }


        #region Scenes
        public static readonly int[] ignoredScenes =
        {
            7, // Main Menu
            10 // Credits
        };
        static (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) getSceneDifference(LevelDescriptor newLevel)
        {
            List<LevelDescriptor> scenesToLoad = new List<LevelDescriptor>(newLevel.LoadLevels);
            List<Scene> scenesToUnload = new List<Scene>();

            // for each new scene
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (GameManager.ignoredScenes.Contains(scene.buildIndex)
                    || ignoredScenes.Contains(scene.buildIndex)) continue;

                LevelDescriptor matchingLevel = null;

                // find the matching Level if it exists
                foreach (LevelDescriptor sceneToLoad in scenesToLoad)
                {
                    if (sceneToLoad.BuildIndex == scene.buildIndex)
                    {
                        matchingLevel = sceneToLoad;
                    }
                }
                if (matchingLevel != null)
                {
                    // Level already exists, so we don't need to load it
                    scenesToLoad.Remove(matchingLevel);
                }
                else
                {
                    // Level no longer exists. so we need to get rid of it
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

        public void TransitionAtmosphere(LevelDescriptor newLevel, bool instant)
        {
            TransitionAtmosphere(newLevel.PostProcessingPrefab, instant);
        }

        public void TransitionAtmosphere(GameObject Prefab, bool instant)
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
