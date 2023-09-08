using Pogo.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class GameProgressTracker
    {
        private PogoGameManager gameManager;
        private int trackedDeaths;
        private float startTime;

        public int TrackedDeaths { get => trackedDeaths; set => trackedDeaths = value; }
        public TimeSpan TrackedTime => TimeSpan.FromSeconds(Time.time - startTime);
        public int TrackedTimeMilliseconds => (int)TrackedTime.TotalMilliseconds;

        public GameProgressTracker(PogoGameManager _gameManager)
        {
            gameManager = _gameManager;
            startTime = Time.time;

            gameManager.OnPlayerDeath.AddListener(GameManager_OnPlayerDeath);
        }

        public GameProgressTracker(PogoGameManager _gameManager, QuickSaveData quickSaveData) : this(_gameManager)
        {
            startTime = Time.time - quickSaveData.ElapsedMilliseconds / 1000;
            trackedDeaths = quickSaveData.TrackedDeaths;
        }

        private void GameManager_OnPlayerDeath()
        {
            TrackedDeaths++;
        }
    }
}
