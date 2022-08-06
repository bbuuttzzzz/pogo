using Inputter;
using System;
using UnityEngine;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Assets.Scripts.Pogo
{
    public class CreditsController : MonoBehaviour
    {
        Animator animator;
        AudioSource audioSource;
        public float CreditsSpeedupMultiplier = 2;
        public float CreditsSpeedupMusicPitch = 1.5f;

        bool ReturnToMainMenuMode = false;

        public ControlSceneDescriptor MainMenuScene;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
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

        private void ReturnToMainMenu()
        {
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
