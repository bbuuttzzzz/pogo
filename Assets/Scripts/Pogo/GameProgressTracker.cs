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
        }

        public GameProgressTracker(PogoGameManager _gameManager, long milliseconds, int deaths) : this(_gameManager)
        {
            startTime = Time.time - (milliseconds / 1000);
            trackedDeaths = deaths;
        }

        public void TrackDeath()
        {
            TrackedDeaths++;
        }
    }
}
