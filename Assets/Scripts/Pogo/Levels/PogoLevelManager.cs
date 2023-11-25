using Pogo.Levels.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Pogo.Levels
{
    public class PogoLevelManager : MonoBehaviour
    {
        public bool LoadInitialLevelImmediately = true;
        public LevelShareCodeManifest ShareCodeManifest;

        private List<LevelSceneLoader> CurrentLevelSceneLoaders;
        private LevelLoadingSettings? CurrentLevelLoadSettings;

        void Start()
        {
            CurrentLevelSceneLoaders = new List<LevelSceneLoader>();
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
            TransitionAtmosphere(level, true);
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
            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = GetSceneDifference(newLevel);

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
            FindFirstObjectByType<PogoGameManager>()._CachedCheckpoint = null;
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

            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = GetSceneDifference(newLevel);

            foreach (LevelDescriptor descriptor in scenesToLoad)
            {
                SceneManager.LoadScene(descriptor.BuildIndex, LoadSceneMode.Additive);
            }

            foreach (Scene scene in scenesToUnload)
            {
                SceneManager.UnloadSceneAsync(scene);
            }

            TransitionAtmosphere(newLevel, true);

            foreach (var initialLevelState in newLevel.LoadLevelStates)
            {
                PogoGameManager.PogoInstance.TryInitializeLevelStateForLevel(initialLevelState);
            }
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
                if (settings.MainLevelState.HasValue)
                {
                    PogoGameManager.PogoInstance.SetLevelState(settings.MainLevelState.Value, settings.Instantly);
                }
                return false;
            }
            currentLevel = settings.Level;

            (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) = GetSceneDifference(settings.Level, CurrentLevelSceneLoaders);

            foreach (var sceneLevel in scenesToLoad)
            {
                var loader = CurrentLevelSceneLoaders.Find(l => l.Level == sceneLevel);

                if (loader == null)
                {
                    loader = new LevelSceneLoader(this, sceneLevel, false);
                    CurrentLevelSceneLoaders.Add(loader);
                }
                loader.MarkNeeded();
                loader.OnReadyToActivate.AddListener(RecalculateFinishedLoadingLevel);
                loader.OnIdle.AddListener(RecalculateFinishedLoadingLevel);
            }

            foreach(var scene in scenesToUnload)
            {
                var loader = CurrentLevelSceneLoaders.Find(l => l.Level.BuildIndex == scene.buildIndex);

                if (loader == null)
                {
                    LevelDescriptor level = FindLevelBySceneBuildIndex(scene.buildIndex);
                    if (level == null)
                    {
                        Debug.LogError($"LoadLevelAsync Failed to find level with buildIndex {scene.buildIndex}. We are going to unload it unsafely!");
                        SceneManager.UnloadSceneAsync(scene);
                        continue;
                    }
                    loader = new LevelSceneLoader(this, level, true);
                    CurrentLevelSceneLoaders.Add(loader);
                }
                loader.MarkNotNeeded(settings.Instantly);
            }

            CurrentLevelLoadSettings = settings;
            RecalculateFinishedLoadingLevel();

            return true;
        }

        private void RecalculateFinishedLoadingLevel()
        {
            for (int i = CurrentLevelSceneLoaders.Count - 1; i >= 0; i--)
            {
                LevelSceneLoader loader = CurrentLevelSceneLoaders[i];
                if (loader.IsIdle)
                {
                    loader.OnReadyToActivate.RemoveAllListeners();
                    loader.OnIdle.RemoveAllListeners();
                    CurrentLevelSceneLoaders.RemoveAt(i);
                }
            }

            if (AllLoadingLevelsAreReady())
            {
                ActivateAllRemainingLoaders();
            }

            if (AllLoadingLevelsFinished())
            {
                FinishLoadingLevel();
            }
        }

        private void ActivateAllRemainingLoaders()
        {
            foreach (var sceneLoader in CurrentLevelSceneLoaders)
            {
                if (sceneLoader.CurrentLoadState == LevelSceneLoader.LoadStates.Loading)
                {
                    sceneLoader.AllowSceneActivation = true;
                }
            }
        }

        private bool AllLoadingLevelsAreReady()
        {
            foreach (var sceneLoader in CurrentLevelSceneLoaders)
            {
                if (sceneLoader.CurrentLoadState == LevelSceneLoader.LoadStates.Loading
                    && sceneLoader.TaskProgress < 0.9f)
                {
                    return false;
                }
            }

            return true;
        }
        private bool AllLoadingLevelsFinished()
        {
            foreach (var sceneLoader in CurrentLevelSceneLoaders)
            {
                if (sceneLoader.CurrentLoadState == LevelSceneLoader.LoadStates.Loading)
                {
                    return false;
                }
            }

            return true;
        }


        private void FinishLoadingLevel()
        {
            if (!CurrentLevelLoadSettings.HasValue) return;
            LevelLoadingSettings settings = CurrentLevelLoadSettings.Value;

            TransitionAtmosphere(CurrentLevel, settings.Instantly);
            PogoGameManager.PogoInstance.OnLevelLoaded?.Invoke();
            if (settings.MainLevelState.HasValue)
            {
                PogoGameManager.PogoInstance.SetLevelState(settings.MainLevelState.Value, settings.Instantly);
            }

            if (settings.AdditionalDefaultLevelStates != null)
            {
                foreach (var initialLevelState in settings.AdditionalDefaultLevelStates)
                {
                    PogoGameManager.PogoInstance.TryInitializeLevelStateForLevel(initialLevelState);
                }
            }
            foreach (var initialLevelState in settings.Level.LoadLevelStates)
            {
                PogoGameManager.PogoInstance.TryInitializeLevelStateForLevel(initialLevelState);
            }

            if (settings.LoadingFromMenu)
            {
                PogoGameManager.PogoInstance.UnloadControlScene();
                PogoGameManager.PogoInstance.ResetStats();
            }

            CurrentLevelLoadSettings = null;
        }

        internal void ResetLoadedLevel()
        {
            currentLevel = null;
        }

        #region Scenes

        private LevelDescriptor FindLevelBySceneBuildIndex(int buildIndex)
        {
            return Array.Find(ShareCodeManifest.Levels, l => l.BuildIndex == buildIndex);
        }

        public static readonly int[] ignoredScenes =
        {
            7, // Main Menu
            10 // Credits
        };

        static (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) GetSceneDifference(LevelDescriptor newLevel,
            IEnumerable<LevelSceneLoader> loaders = null)
        {
#if DEBUG
            var solidLevels = newLevel.LoadLevels.ToList();
            for (int i = solidLevels.Count - 1; i >= 0; i--)
            {
                LevelDescriptor level = solidLevels[i];
                if (level == null)
                {
                    Debug.LogWarning($"One of the Adjacent levels for {newLevel} is NULL!!! (index {i})");
                    solidLevels.RemoveAt(i);
                }
            }
            return GetSceneDifference(solidLevels, loaders ?? new LevelSceneLoader[0]);
#else
            return GetSceneDifference(newLevel.LoadLevels, loaders ?? new LevelSceneLoader[0]);
#endif
        }

        static (List<LevelDescriptor> scenesToLoad, List<Scene> scenesToUnload) GetSceneDifference(
            IEnumerable<LevelDescriptor> loadLevels,
            IEnumerable<LevelSceneLoader> loaders
            )
        {
            List<LevelDescriptor> scenesToLoad = new List<LevelDescriptor>(loadLevels);
            List<Scene> scenesToUnload = new List<Scene>();

            // for each currently loaded scene
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                
                // just ignore the GameScene, Main Menu, and Credits scenes
                if (GameManager.ignoredScenes.Contains(scene.buildIndex)
                    || ignoredScenes.Contains(scene.buildIndex)) continue;

                LevelDescriptor matchingToLoadLevel = null;
                foreach (LevelDescriptor sceneToLoad in scenesToLoad)
                {
                    if (sceneToLoad.BuildIndex == scene.buildIndex)
                    {
                        matchingToLoadLevel = sceneToLoad;
                    }
                }

                // if we want to have it loaded
                if (matchingToLoadLevel != null)
                {
                    LevelSceneLoader existingLoader = loaders.FirstOrDefault(l => l.Level == matchingToLoadLevel);

                    // if this level is marked as not needed
                    if (existingLoader != null && !existingLoader.CurrentlyNeeded)
                    {
                        // keep it in our scenesToLoad list, so we know to mark it needed
                    }
                    else
                    {
                        // Level already exists, so we don't need to load it
                        scenesToLoad.Remove(matchingToLoadLevel);
                    }
                }
                else
                {
                    // Level no longer exists. so we need to get rid of it
                    scenesToUnload.Add(scene);
                }
            }

            // for each unloading scene
            for (int i = scenesToUnload.Count - 1; i >= 0; i--)
            {
                Scene scene = scenesToUnload[i];
                LevelSceneLoader existingLoader = loaders.FirstOrDefault(l => l.Level.BuildIndex == scene.buildIndex);

                // if this level is marked as needed
                if (existingLoader != null && existingLoader.CurrentlyNeeded)
                {
                    // we will want to mark it as not needed
                    scenesToUnload.RemoveAt(i);
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
        [ContextMenu("Log LevelLoader Status")]
        public void LogLoaders()
        {
            StringBuilder sb = new StringBuilder();
            CurrentLevelSceneLoaders.ForEach(loader => sb.AppendLine(loader.ToString()));

            Debug.Log(sb.ToString());
        }
    }
}
