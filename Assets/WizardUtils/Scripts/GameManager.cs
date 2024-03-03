using Inputter;
using Platforms;
using Pogo.Levels.Loading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WizardUtils.GameSettings;
using WizardUtils.Saving;
using WizardUtils.SceneManagement;

namespace WizardUtils
{
    public abstract class GameManager : MonoBehaviour
    {
        public static GameManager GameInstance;
        public IPlatformService PlatformService;
        [NonSerialized]
        public UnityEvent OnQuitToMenu = new UnityEvent();
        [NonSerialized]
        public UnityEvent OnQuitToDesktop = new UnityEvent();
        public string SaveDataPath => PlatformService.SaveDataPath;
        public string PersistentDataPath => PlatformService.PersistentDataPath;
        public AudioManager AudioManager => GetComponent<AudioManager>();


        protected virtual void Awake()
        {
            if (GameInstance != null)
            {
                Destroy(this);
                return;
            }

            GameInstance = this;
            DontDestroyOnLoad(gameObject);

#if !DISABLESTEAMWORKS
            PlatformService = new Platforms.Steam.SteamPlatformService();
#else
            PlatformService = new Platforms.Portable.PortablePlatformService();
#endif
            GameSettingService = PlatformService.BuildGameSettingService(LoadGameSettings());

            CurrentSceneLoaders = new List<SceneLoader>();

            SetupSaveData();
        }

        protected virtual List<GameSettingFloat> LoadGameSettings()
        {
            return new List<GameSettingFloat>()
            {
                new GameSettingFloat(KEY_VOLUME_MASTER, 100),
                new GameSettingFloat(KEY_VOLUME_EFFECTS, 80),
                new GameSettingFloat(KEY_VOLUME_AMBIENCE, 80),
                new GameSettingFloat(KEY_VOLUME_MUSIC, 80),
                new GameSettingFloat(SETTINGKEY_MUTE_ON_ALT_TAB, 0),
            };
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnEnable()
        {
            PlatformService?.OnEnable();
        }

        protected virtual void OnDestroy()
        {
            PlatformService?.OnDestroy();
        }

        protected virtual void OnApplicationQuit()
        {
            GameSettingService.Save();
            OnQuitToDesktop?.Invoke();
        }

        public static bool GameInstanceIsValid()
        {
            return GameInstance != null;
        }

        #region Pausing
        public virtual bool LockPauseState => false;

        bool paused;

#if UNITY_EDITOR
        public bool BreakOnPause;
#endif

        public bool Paused
        {
            get => paused;
            set
            {
                if (!GameInstanceIsValid()) return;

                if (paused == value || GameInstance.LockPauseState) return;
                paused = value;

#if UNITY_EDITOR
                if (value && GameInstance.BreakOnPause) Debug.Break();
#endif
                GameInstance.OnPauseStateChanged?.Invoke(null, value);
            }
        }
        public EventHandler<bool> OnPauseStateChanged;
        #endregion

        #region Scenes
        public static readonly int[] ignoredScenes =
        {
            0, // this is GameScene
        };

        public EventHandler<ControlSceneEventArgs> OnControlSceneChanged;
        public bool InControlScene => CurrentControlScene != null;
        public bool InGameScene => CurrentControlScene == null;

        private List<SceneLoader> CurrentSceneLoaders;
        public ControlSceneDescriptor MainMenuControlScene;
        [HideInInspector]
        public ControlSceneDescriptor CurrentControlScene;
        private SceneLoadingData CurrentSceneLoadingData;

#if UNITY_EDITOR
        public void UnloadControlSceneInEditor()
        {
            var initialScene = CurrentControlScene;

            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                for (int n = 0; n < SceneManager.sceneCount; n++)
                {
                    Scene scene = SceneManager.GetSceneAt(n);
                    if (scene.buildIndex == CurrentControlScene.BuildIndex)
                    {

                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                        CurrentControlScene = null;
                        break;
                    }
                }
                OnControlSceneChanged?.Invoke(this, new ControlSceneEventArgs(initialScene, null));
            }
        }

        public virtual void LoadControlSceneInEditor(ControlSceneDescriptor newScene)
        {
            bool newSceneAlreadyLoaded = false;
            List<Scene> scenesToUnload = new List<Scene>();
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (!ignoredScenes.Contains(scene.buildIndex))
                {
                    if (scene.buildIndex == newScene.BuildIndex)
                    {
                        newSceneAlreadyLoaded = true;
                    }
                    else
                    {
                        scenesToUnload.Add(scene);
                    }
                }
            }

            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // load the control scene
                if (!newSceneAlreadyLoaded)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(newScene.ScenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                }
                CurrentControlScene = newScene;

                // unload all non-ignored scenes
                foreach (Scene scene in scenesToUnload)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
#endif

        public virtual void LoadControlSceneAsync(ControlSceneDescriptor newControlScene, Action callback = null)
        {
            if (DontLoadScenesInEditor) return;
            var initialScene = CurrentControlScene;

            if (CurrentSceneLoadingData != null)
            {
                Debug.LogWarning($"Overriding old ControlScene Loading Data {CurrentSceneLoadingData.InitialScene.name} -> {CurrentSceneLoadingData.FinalScene.name}");
            }
            CurrentSceneLoadingData = new SceneLoadingData()
            {
                InitialScene = initialScene,
                FinalScene = newControlScene,
                Callback = callback
            };

            (List <Scene> scenesToLoad, List<Scene> scenesToUnload) = GetSceneDifference(SceneManager.GetSceneByBuildIndex(newControlScene.BuildIndex), CurrentSceneLoaders);

            foreach(var scene in scenesToLoad)
            {
                var loader = CurrentSceneLoaders.Find(l => l.Scene.buildIndex == scene.buildIndex);
                if (loader == null)
                {
                    loader = new SceneLoader(this, scene, false);
                }
                loader.MarkNeeded();
                loader.OnReadyToActivate.AddListener(RecalculateFinishedLoadingControlScene);
                loader.OnIdle.AddListener(RecalculateFinishedLoadingControlScene);
            }

            foreach(var scene in scenesToUnload)
            {
                var loader = CurrentSceneLoaders.Find(l => l.Scene.buildIndex == scene.buildIndex);

                if (loader == null)
                {
                    loader = new SceneLoader(this, scene, true);
                    CurrentSceneLoaders.Add(loader);
                }
                loader.MarkNotNeeded();
                loader.OnReadyToActivate.AddListener(RecalculateFinishedLoadingControlScene);
                loader.OnIdle.AddListener(RecalculateFinishedLoadingControlScene);
            }

            CurrentControlScene = newControlScene;
        }

        private void FinishLoadingControlScene()
        {
            CurrentSceneLoadingData.Callback?.Invoke();
            OnControlSceneChanged?.Invoke(this, new ControlSceneEventArgs(CurrentSceneLoadingData.InitialScene, CurrentSceneLoadingData.FinalScene));
            CurrentSceneLoadingData = null;
        }

        private void RecalculateFinishedLoadingControlScene()
        {
            for (int i = CurrentSceneLoaders.Count - 1; i >= 0; i--)
            {
                SceneLoader loader = CurrentSceneLoaders[i];
                if (loader.IsIdle)
                {
                    loader.OnReadyToActivate.RemoveAllListeners();
                    loader.OnIdle.RemoveAllListeners();
                    CurrentSceneLoaders.RemoveAt(i);
                }
            }

            if (AllLoadingLevelsAreReady())
            {
                ActivateAllRemainingLoaders();
            }

            if (AllLoadingLevelsFinished())
            {
                FinishLoadingControlScene();
            }
        }

        private void ActivateAllRemainingLoaders()
        {
            foreach (var sceneLoader in CurrentSceneLoaders)
            {
                if (sceneLoader.CurrentLoadState == SceneLoader.LoadStates.Loading)
                {
                    sceneLoader.AllowSceneActivation = true;
                }
            }
        }

        private bool AllLoadingLevelsAreReady()
        {
            foreach (var sceneLoader in CurrentSceneLoaders)
            {
                if (sceneLoader.CurrentLoadState == SceneLoader.LoadStates.Loading
                    && sceneLoader.TaskProgress < 0.9f)
                {
                    return false;
                }
            }

            return true;
        }
        private bool AllLoadingLevelsFinished()
        {
            foreach (var sceneLoader in CurrentSceneLoaders)
            {
                if (sceneLoader.CurrentLoadState == SceneLoader.LoadStates.Loading)
                {
                    return false;
                }
            }

            return true;
        }

        private static (List<Scene> scenesToLoad, List<Scene> scenesToUnload) GetSceneDifference(
            Scene newScene,
            IEnumerable<SceneLoader> loaders = null)
        {
            List<Scene> scenesToLoad = new List<Scene>();
            List<Scene> scenesToUnload = new List<Scene>();

            scenesToLoad.Add(newScene);

            // for each currently loaded scene
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);

                // just ignore the GameScene, Main Menu, and Credits scenes
                if (GameManager.ignoredScenes.Contains(scene.buildIndex)
                    || ignoredScenes.Contains(scene.buildIndex)) continue;

                Scene? matchingToLoadScene = null;
                foreach (Scene sceneToLoad in scenesToLoad)
                {
                    if (sceneToLoad.buildIndex == scene.buildIndex)
                    {
                        matchingToLoadScene = sceneToLoad;
                    }
                }

                // if we want to have it loaded
                if (matchingToLoadScene != null)
                {
                    SceneLoader existingLoader = loaders.FirstOrDefault(l => l.Scene.buildIndex == matchingToLoadScene.Value.buildIndex);

                    // if this level is marked as not needed
                    if (existingLoader != null && !existingLoader.CurrentlyNeeded)
                    {
                        // keep it in our scenesToLoad list, so we know to mark it needed
                    }
                    else
                    {
                        // Scene already exists, so we don't need to load it
                        scenesToLoad.Remove(matchingToLoadScene.Value);
                    }
                }
                else
                {
                    // Scene no longer exists. so we need to get rid of it
                    scenesToUnload.Add(scene);
                }
            }

            return (scenesToLoad, scenesToUnload);
        }

        IEnumerator AfterTasksFinish(List<AsyncOperation> tasks, Action callback)
        {
            bool finished = false;
            while (!finished)
            {
                finished = true;
                foreach (AsyncOperation Task in tasks)
                {
                    finished = finished && Task.isDone;
                }

                yield return new WaitForSecondsRealtime(0.02f);
            }

            callback?.Invoke();
        }

        public void UnloadControlScene()
        {
            SceneManager.UnloadSceneAsync(CurrentControlScene.BuildIndex);
            var initialScene = CurrentControlScene;
            CurrentControlScene = null;
            OnControlSceneChanged?.Invoke(this, new ControlSceneEventArgs(initialScene, null));
        }

        public void Quit(bool quitToDesktop)
        {
            if (quitToDesktop)
            {
                GameSettingService.Save();
                OnQuitToDesktop?.Invoke();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
                Debug.LogError("Can't quit in web builds!!! D:");
#else
                Application.Quit();
#endif
            }
            else
            {
                OnQuitToMenu?.Invoke();
                LoadControlSceneAsync(MainMenuControlScene);
            }
        }
        #endregion

        #region GameSettings
        IGameSettingService GameSettingService;

        public bool DontLoadScenesInEditor;

        public void SaveSettingsChanges()
        {
            GameSettingService.Save();
        }

        public GameSettingFloat FindGameSetting(string key)
        {
            return GameSettingService.GetSetting(key);
        }

        public static string KEY_VOLUME_MASTER = "Volume_Master";
        public static string KEY_VOLUME_EFFECTS = "Volume_Effects";
        public static string KEY_VOLUME_AMBIENCE = "Volume_Ambience";
        public static string KEY_VOLUME_MUSIC = "Volume_Music";

        public static string SETTINGKEY_MUTE_ON_ALT_TAB = "MuteOnAltTab";
        #endregion

        #region Saving
        public SaveManifest MainSaveManifest;
        public ExplicitSaveData EditorOverrideSaveData;
        public bool DontSaveInEditor;
        SaveDataTracker saveDataTracker;

        private void SetupSaveData()
        {
            if (MainSaveManifest == null) return;

#if UNITY_EDITOR
            if (EditorOverrideSaveData != null)
            {
                saveDataTracker = new SaveDataTrackerExplicit(MainSaveManifest, EditorOverrideSaveData);
            }
            else
            {
                saveDataTracker = new SaveDataTrackerFile(PlatformService, MainSaveManifest);
            }
#else
            saveDataTracker = new SaveDataTrackerFile(PlatformService, MainSaveManifest);
#endif
            saveDataTracker.Load();
        }

        public string GetMainSaveValue(SaveValueDescriptor descriptor)
        {
#if UNITY_EDITOR
            if (saveDataTracker == null)
            {
                Debug.LogWarning("Tried so load data without a MainSaveManifest", this);
            }
#endif
            return saveDataTracker?.GetSaveValue(descriptor)?? null;
        }

        public void SetMainSaveValue(SaveValueDescriptor descriptor, string stringValue)
        {

#if UNITY_EDITOR
            if (saveDataTracker == null)
            {
                Debug.LogWarning("Tried so save data without a MainSaveManifest", this);
            }
#endif
            saveDataTracker?.SetSaveValue(descriptor, stringValue);
        }

        public void SaveData()
        {
            saveDataTracker.Save();
        }
        #endregion
    }
}