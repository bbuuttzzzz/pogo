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
        public float TrackedTime => Time.time - startTime;
        public int TrackedTimeMilliseconds => (int)(TrackedTime * 1000);

        public GameProgressTracker(PogoGameManager _gameManager)
        {
            gameManager = _gameManager;
            startTime = Time.time;

            gameManager.OnPlayerDeath.AddListener(GameManager_OnPlayerDeath);
        }

        private void GameManager_OnPlayerDeath()
        {
            TrackedDeaths++;
        }
    }
}
