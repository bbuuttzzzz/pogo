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
    public class GameManager : MonoBehaviour
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
        }

        protected virtual void Update()
        {

        }

        public static bool GameInstanceIsValid()
        {
            return GameInstance != null;
        }

        #region Pausing

        public virtual bool CanPause => true;

        bool paused;

        public static bool Paused
        {
            get => GameInstance?.paused ?? false;
            set
            {
                if (!GameInstanceIsValid()) return;

                if (GameInstance.paused == value || !GameInstance.CanPause) return;
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
            throw new NotImplementedException();
        }
        #endregion
    }
}