using Pogo;
using Pogo.Difficulties;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo.PauseMenu
{
    public class PauseMenuFloaterButtonsTab : MonoBehaviour
    {
        public Transform ButtonsRoot;
        public PauseMenuController pauseMenuController;
        public GameObject SkipButtonPrefab;
        public GameObject RestartButtonPrefab;

        private PauseMenuFloaterButton[] pauseMenuFloaterButtons;
        private GameSettingFloat ShowRestartSetting;
        private void Start()
        {
            pauseMenuFloaterButtons = new[]
            {
                MakeSkipButton(), // we use this index in Skip()
                MakeRestartButton()
            };
            GameManager.GameInstance.OnPauseStateChanged += onPauseStateChanged;
            ShowRestartSetting = PogoGameManager.PogoInstance.FindGameSetting(PogoGameManager.SETTINGKEY_SHOWRESTART);
        }

        protected virtual void onPauseStateChanged(object sender, bool nowPaused)
        {
            if (nowPaused)
            {
                Refresh();
            }
        }


        private PauseMenuFloaterButton MakeSkipButton()
        {
            var button = new PauseMenuFloaterButton()
            {
                Button = Instantiate(SkipButtonPrefab, ButtonsRoot).GetComponent<Button>(),
                GetDisplayType = GetSkipButtonDisplayType
            };

            button.Button.onClick.AddListener(Skip);

            return button;
        }

        private PauseMenuFloaterButton.DisplayTypes GetSkipButtonDisplayType()
        {
            if (PogoGameManager.PogoInstance.CurrentDifficulty != Difficulty.Assist)
            {
                return PauseMenuFloaterButton.DisplayTypes.Hidden;
            }

            if (PogoGameManager.PogoInstance.CanSkipCheckpoint())
            {
                return PauseMenuFloaterButton.DisplayTypes.Enabled;
            }

            return PauseMenuFloaterButton.DisplayTypes.Disabled;
        }

        private PauseMenuFloaterButton MakeRestartButton()
        {
            var button = new PauseMenuFloaterButton()
            {
                Button = Instantiate(RestartButtonPrefab, ButtonsRoot).GetComponent<Button>(),
                GetDisplayType = GetRestartButtonDisplayType
            };

            button.Button.GetComponent<RestartButtonController>().OnTrigger.AddListener(Restart);

            return button;
        }

        private PauseMenuFloaterButton.DisplayTypes GetRestartButtonDisplayType()
        {
            if (PogoGameManager.PogoInstance.CustomMapBuilder.CurrentCustomMap != null)
                return PauseMenuFloaterButton.DisplayTypes.Enabled;

            if (ShowRestartSetting.Value == 1)
            {
                return PauseMenuFloaterButton.DisplayTypes.Enabled;
            }

            return PauseMenuFloaterButton.DisplayTypes.Hidden;
        }

        private void Restart()
        {
            if (PogoGameManager.PogoInstance.CustomMapBuilder.CurrentCustomMap != null)
            {
                PogoGameManager.PogoInstance.CustomMapBuilder.RestartMap();
            }
            else if (ShowRestartSetting.Value == 1)
            {
                PogoGameManager.PogoInstance.QuickRestart();
            }
            PogoGameManager.PogoInstance.Paused = false;
        }

        public void Refresh()
        {
            foreach(var button in pauseMenuFloaterButtons)
            {
                var state = button.GetDisplayType();
                button.Button.gameObject.SetActive(state != PauseMenuFloaterButton.DisplayTypes.Hidden);
                button.Button.interactable = state != PauseMenuFloaterButton.DisplayTypes.Disabled;
            }
        }

        public void Skip()
        {
            if (!PogoGameManager.PogoInstance.TrySkipCheckpoint())
            {
                Debug.LogError($"Failed to skip CurrentCheckpoint: {PogoGameManager.PogoInstance.CurrentCheckpoint}");
                pauseMenuFloaterButtons[0].Button.interactable = false;
                return;
            }

            pauseMenuController.Resume();
        }
    }
}