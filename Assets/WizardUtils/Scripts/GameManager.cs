using Inputter;
using Platforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string PersistentDataPath => PlatformService.PersistentDataPath;

        protected virtual void Awake()
        {
            if (GameInstance != null)
            {
                Destroy(this);
                return;
            }

            GameInstance = this;
            DontDestroyOnLoad(gameObject);

#if STORE_STEAM
            PlatformService = new Platforms.Steam.SteamPlatformService();
#else
            PlatformService = new Platforms.Portable.PortablePlatformService();
#endif
            GameSettingService = PlatformService.BuildGameSettingService(LoadGameSettings());

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

        public ControlSceneDescriptor MainMenuControlScene;
        [HideInInspector]
        public ControlSceneDescriptor CurrentControlScene;

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
                    CurrentControlScene = newScene;
                }

                // unload all non-ignored scenes
                foreach (Scene scene in scenesToUnload)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
#endif

        public virtual void LoadControlScene(ControlSceneDescriptor newScene, Action<List<AsyncOperation>> callback = null)
        {
            if (DontLoadScenesInEditor) return;
            var initialScene = CurrentControlScene;
            List<AsyncOperation> tasks = new List<AsyncOperation>();

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

            // load the control scene
            if (!newSceneAlreadyLoaded)
            {
                tasks.Add(SceneManager.LoadSceneAsync(newScene.BuildIndex, LoadSceneMode.Additive));
                CurrentControlScene = newScene;
            }

            // unload all non-ignored scenes
            foreach (Scene scene in scenesToUnload)
            {
                AsyncOperation task = SceneManager.UnloadSceneAsync(scene);
                if (task != null)
                {
                    tasks.Add(task);
                }
                else
                {
                    Debug.LogWarning($"error unloading scene {scene.name}");
                }
            }

            CurrentControlScene = newScene;
            OnControlSceneChanged?.Invoke(this, new ControlSceneEventArgs(initialScene, newScene));
            callback?.Invoke(tasks);
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
                LoadControlScene(MainMenuControlScene);
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
        #endregion

        #region Saving
        public SaveManifest MainSaveManifest;
        public ExplicitSaveData EditorOverrideSaveData;
        public bool DontSaveInEditor;
        SaveDataTracker saveDataTracker;
        private bool SaveDataOpen;

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
            SaveDataOpen = true;
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