using Pogo;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Logic
{
    public class XDeathsListener : Listener
    {
        [Tooltip("Called after this # of deaths")]
        public int Deaths;
        public bool RepeatEachDeath;

        bool heard;
        int deathcount;
        private void CountDeath()
        {
            if (++deathcount > Deaths && !heard || RepeatEachDeath)
            {
                heard = true;
                Heard();
            }
        }


        protected override void Listen()
        {
            deathcount = 0;
            heard = false;
            PogoGameManager.PogoInstance?.OnPlayerDeath.AddListener(CountDeath);
        }

        protected override void StopListening()
        {
            PogoGameManager.PogoInstance?.OnPlayerDeath.RemoveListener(CountDeath);
        }
    }
}
