using Pogo.Difficulties;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo.Logic
{
    public class StatsHUDController : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public int QuickDisplayInterval = 10;

        public bool ShowTimerSetting;
        public bool HideStats;

        public MeshFilter SkullMesh;
        public Renderer SkullMeshRenderer;

        Animator animator;

        private void Start()
        {
            gameManager = PogoGameManager.PogoInstance;
            animator = GetComponent<Animator>();
            var showTimerSetting = PogoGameManager.GameInstance.FindGameSetting(PogoGameManager.SETTINGKEY_TIMER);
            showTimerSetting.OnChanged += onShowTimerChanged;
            ShowTimerSetting = showTimerSetting.Value == 1;

            gameManager.OnHideStatsChanged.AddListener(GameManager_OnHideStatsChanged);
            gameManager.OnPlayerDeath.AddListener(onDeath);
            gameManager.OnPauseStateChanged += onPauseStateChanged;
            gameManager.OnStatsReset.AddListener(onStatsReset);
            gameManager.OnDifficultyChanged.AddListener(onDifficultyChanged);
        }

        private void GameManager_OnHideStatsChanged(bool arg0)
        {
            HideStats = arg0;
            updateDisplay();
        }

        private void onShowTimerChanged(object sender, WizardUtils.GameSettingChangedEventArgs e)
        {
            ShowTimerSetting = e.FinalValue == 1;
            updateDisplay();
        }

        private void onStatsReset()
        {
            OnDeathCountChanged?.Invoke(gameManager.CurrentSessionDeaths);
        }
        private void onDifficultyChanged(DifficultyChangedEventArgs e)
        {
            SkullMesh.sharedMesh = gameManager.CurrentDifficultyDescriptor.SkullMesh;
            SkullMeshRenderer.sharedMaterial = gameManager.CurrentDifficultyDescriptor.SkullMaterial;
        }

        private void Update()
        {
            if (gameManager.CurrentGameState != PogoGameManager.GameStates.InGame) return;

            if (ShowTimerSetting && Time.timeScale > 0)
            {
                UpdateStopwatchTimerText();
            }
        }

        private void UpdateStopwatchTimerText()
        {
            var time = gameManager.TrackedSessionTime;
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
            animator.SetBool("DisplayDeaths", !HideStats && isPaused);
            animator.SetBool("DisplayTimer", !HideStats && isPaused && ShowTimerSetting);
            UpdateStopwatchTimerText();
        }

        private void onDeath()
        {
            int deathCount = gameManager.TrackedSessionDeaths;
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
