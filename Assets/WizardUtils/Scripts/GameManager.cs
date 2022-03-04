using Inputter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace WizardUtils
{
    public abstract class GameManager : MonoBehaviour
    {
        public static GameManager GameInstance;
        protected virtual void Awake()
        {
            if (GameInstance != null)
            {
                Destroy(this);
                return;
            }

            GameInstance = this;
            DontDestroyOnLoad(gameObject);
            GameSettings = new List<GameSettingFloat>();

            RegisterGameSetting(new GameSettingFloat(KEY_VOLUME_MASTER, 100));
            RegisterGameSetting(new GameSettingFloat(KEY_VOLUME_EFFECTS, 80));
            RegisterGameSetting(new GameSettingFloat(KEY_VOLUME_AMBIENCE, 80));
        }

        protected virtual void Update()
        {

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

        public static bool Paused
        {
            get => GameInstance?.paused ?? false;
            set
            {
                if (!GameInstanceIsValid()) return;

                if (GameInstance.paused == value || GameInstance.LockPauseState) return;
                GameInstance.paused = value;
                if (value)
                {
                    pause();
                }
                else
                {
                    resume();
                }
                GameInstance.OnPauseStateChanged?.Invoke(null, value);
            }
        }
        public EventHandler<bool> OnPauseStateChanged;

        private static void pause()
        {
#if UNITY_EDITOR
            if (GameInstance.BreakOnPause) Debug.Break();
#endif
            Time.timeScale = 0;
        }

        private static void resume()
        {
            Time.timeScale = 1;
        }
        #endregion

        #region Scenes
        static readonly int[] ignoredScenes =
        {
            0 // this is GameScene
        };

        public EventHandler<ControlSceneEventArgs> OnControlSceneChanged;
        public bool InControlScene => CurrentControlScene != null;
        public bool InGameScene => CurrentControlScene = null;

        public ControlSceneDescriptor MainMenuControlScene;
        [HideInInspector]
        public ControlSceneDescriptor CurrentControlScene;

#if UNITY_EDITOR
        public void LoadControlSceneInEditor(ControlSceneDescriptor newScene)
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

            // load the control scene
            if (!newSceneAlreadyLoaded)
            {
                EditorSceneManager.OpenScene(newScene.ScenePath, OpenSceneMode.Additive);
                CurrentControlScene = newScene;
            }

            // unload all non-ignored scenes
            foreach (Scene scene in scenesToUnload)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
#endif

        public void LoadControlScene(ControlSceneDescriptor newScene, Action<List<AsyncOperation>> callback = null)
        {
            if (DontLoadScenesInEditor) return;
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

            OnControlSceneChanged?.Invoke(this, new ControlSceneEventArgs(newScene));
            callback?.Invoke(tasks);
        }

        public void UnloadControlScene()
        {
            SceneManager.UnloadSceneAsync(CurrentControlScene.BuildIndex);
            CurrentControlScene = null;
            OnControlSceneChanged?.Invoke(this, new ControlSceneEventArgs(null));
        }

        public void Quit(bool hardQuit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            Debug.LogError("Can't quit in web builds!!! D:");
#else
            Application.Quit();
#endif
        }
        #endregion

        #region GameSettings
        List<GameSettingFloat> GameSettings;

        public bool DontLoadScenesInEditor;

        protected void RegisterGameSetting(GameSettingFloat setting)
        {
            GameSettings.Add(setting);
        }

        public GameSettingFloat FindGameSetting(string key)
        {
            foreach(GameSettingFloat setting in GameSettings)
            {
                if (setting.Key == key)
                {
                    return setting;
                }
            }
            throw new KeyNotFoundException($"Missing GameSetting \"{key}\"");
        }

        public static string KEY_VOLUME_MASTER = "Volume_Master";
        public static string KEY_VOLUME_EFFECTS = "Volume_Effects";
        public static string KEY_VOLUME_AMBIENCE = "Volume_Ambience";
#endregion
    }
}