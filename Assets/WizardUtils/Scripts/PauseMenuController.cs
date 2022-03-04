using System;
using UnityEngine;
using UnityEngine.UI;

namespace WizardUtils
{
    public class PauseMenuController : MonoBehaviour
    {
        public GameObject PauseScreen;

        protected virtual void Start()
        {
            GameManager.GameInstance.OnPauseStateChanged += onPauseStateChanged;
        }

        protected virtual void onPauseStateChanged(object sender, bool nowPaused)
        {
            PauseScreen?.SetActive(nowPaused);
        }

        public void Resume()
        {
            GameManager.Paused = false;
        }

        public void ReturnToMainMenu()
        {
            Resume();
            GameManager.GameInstance?.Quit(false);
        }

    }

}
