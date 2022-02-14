using Inputter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

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
        public void Quit(bool hardQuit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL

#else
            Application.Quit();
#endif
        }
#endregion

#region GameSettings
        List<GameSettingFloat> GameSettings;

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
#endregion
    }
}