using Inputter;
using Pogo;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;
using WizardUtils.Saving;
using WizardUtils.SceneManagement;

namespace Assets.Scripts.Pogo
{
    public class CreditsController : MonoBehaviour
    {
        Animator animator;
        AudioSource audioSource;
        public float CreditsSpeedupMultiplier = 2;
        public float CreditsSpeedupMusicPitch = 1.5f;
        public UnityEvent OnBeforeReturnToMainMenu;

        bool ReturnToMainMenuMode = false;

        public ControlSceneDescriptor MainMenuScene;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            UpdateStopwatchTimerText();
            DeathCountText.text = PogoGameManager.PogoInstance.NumberOfDeaths.ToString();
        }

        private void Update()
        {
            if (ReturnToMainMenuMode)
            {
                if (InputManager.CheckKeyDown(KeyName.UIAdvance))
                {
                    ReturnToMainMenu();
                }
            }
            else
            {
                bool speedup = InputManager.CheckKey(KeyName.UIAdvance);
                animator.speed = speedup ? CreditsSpeedupMultiplier : 1;
                audioSource.pitch = speedup ? CreditsSpeedupMusicPitch : 1;
            }
        }

        public GameObject StopwatchObject;
        public Text StopwatchTimerText;
        public Text DeathCountText;

        private void UpdateStopwatchTimerText()
        {
            var time = TimeSpan.FromSeconds(PogoGameManager.FinalTime);
            StopwatchTimerText.text = $"{Math.Floor(time.TotalMinutes)}:{time.Seconds:00}.{time.Milliseconds:000}";
        }

        private void ReturnToMainMenu()
        {
            OnBeforeReturnToMainMenu?.Invoke();
            GameManager.GameInstance.LoadControlScene(MainMenuScene);
        }

        public void EnableReturnToMainMenu()
        {
            ReturnToMainMenuMode = true;
            animator.speed = 1;
            audioSource.pitch = 1;
        }
    }
}
