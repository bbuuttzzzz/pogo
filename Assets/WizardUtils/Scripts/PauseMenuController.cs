using System;
using UnityEngine;
using UnityEngine.UI;

namespace WizardUtils
{
    public class PauseMenuController : MonoBehaviour
    {
        public GameObject PauseScreen;

        public Button QuitButton;
        public Button ResumeButton;


        protected virtual void Start()
        {
            GameManager.GameInstance.OnPauseStateChanged += onPauseStateChanged;
            ResumeButton?.onClick.AddListener(onResumeClicked);
            QuitButton?.onClick.AddListener(onQuitClicked);
        }

        protected virtual void onPauseStateChanged(object sender, bool nowPaused)
        {
            PauseScreen?.SetActive(nowPaused);
        }


        private void onResumeClicked()
        {
            GameManager.Paused = false;
        }

        private void onQuitClicked()
        {
            GameManager.GameInstance?.Quit(false);
        }

    }

}
