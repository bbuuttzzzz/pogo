using System;
using TMPro;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapClearedMenu : ToggleableUIElement
    {
        private PogoGameManager gameManager;

        public Button RestartButton;
        public Button QuitButton;
        public IntegerFormatter DeathsFormatter;
        public TextMeshProUGUI StopwatchText;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;
        }

        private void Start()
        {
            OnOpen.AddListener(Base_OnOpen);
            OnClose.AddListener(Base_OnClose);
            RestartButton.onClick.AddListener(Restart);
            QuitButton.onClick.AddListener(Quit);
        }

        private void Restart()
        {
            gameManager.CustomMapBuilder.RestartMap();
        }

        private void Quit()
        {
            gameManager.Paused = false;
            gameManager.Quit(false);
        }


        private void Base_OnOpen()
        {
            gameManager.HideStatsPopup = true;
            DeathsFormatter.FormatInt(gameManager.CustomMapBuilder.LastAttemptData.Deaths);
            var time = TimeSpan.FromMilliseconds(gameManager.CustomMapBuilder.LastAttemptData.CompletionTimeMS);
            StopwatchText.text = $"{Math.Floor(time.TotalMinutes)}:{time.Seconds:00}.{time.Milliseconds:000}";

        }

        private void Base_OnClose()
        {
            gameManager.HideStatsPopup = false;
        }
    }
}
