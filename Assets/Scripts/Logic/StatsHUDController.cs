using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo.Logic
{
    public class StatsHUDController : MonoBehaviour
    {
        public int QuickDisplayInterval = 10;

        public bool ShouldShowStopwatch;

        public MeshFilter SkullMesh;
        public Renderer SkullMeshRenderer;

        Animator animator;
        private void Start()
        {
            animator = GetComponent<Animator>();
            var showTimerSetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.SETTINGKEY_TIMER);
            showTimerSetting.OnChanged += onShowTimerChanged;
            ShouldShowStopwatch = showTimerSetting.Value == 1;

            PogoGameManager.PogoInstance.OnPlayerDeath.AddListener(onDeath);
            PogoGameManager.PogoInstance.OnPauseStateChanged += onPauseStateChanged;
            PogoGameManager.PogoInstance.OnStatsReset.AddListener(onStatsReset);
            PogoGameManager.PogoInstance.OnDifficultyChanged.AddListener(onDifficultyChanged);
        }


        private void onShowTimerChanged(object sender, WizardUtils.GameSettingChangedEventArgs e)
        {
            ShouldShowStopwatch = e.FinalValue == 1;
            updateDisplay();
        }

        private void onStatsReset()
        {
            OnDeathCountChanged?.Invoke(PogoGameManager.PogoInstance.CurrentSessionDeaths);
        }
        private void onDifficultyChanged(DifficultyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            if (PogoGameManager.PogoInstance.CurrentGameState != PogoGameManager.GameStates.InGame) return;

            if (ShouldShowStopwatch && Time.timeScale > 0)
            {
                UpdateStopwatchTimerText();
            }
        }

        private void UpdateStopwatchTimerText()
        {
            var time = PogoGameManager.PogoInstance.TrackedSessionTime;
            StopwatchTimerText.text = $"{Math.Floor(time.TotalMinutes)}:{time.Seconds:00}.{time.Milliseconds:000}";
        }

        bool isPaused;
        private void onPauseStateChanged(object sender, bool e)
        {
            isPaused = e;
            updateDisplay();
        }

        private void updateDisplay()
        {
            animator.SetBool("DisplayDeaths", isPaused);
            animator.SetBool("DisplayTimer", isPaused && ShouldShowStopwatch);
            UpdateStopwatchTimerText();
        }

        private void onDeath()
        {
            int deathCount = PogoGameManager.PogoInstance.TrackedSessionDeaths;
            OnDeathCountChanged?.Invoke(deathCount);
            if (deathCount % QuickDisplayInterval == 0)
            {
                OnDeathCountChangedLargeInterval?.Invoke();
            }
        }

        public UnityEvent<int> OnDeathCountChanged;
        public UnityEvent OnDeathCountChangedLargeInterval;

        public GameObject StopwatchObject;
        public TextMeshProUGUI StopwatchTimerText;
    }
}
