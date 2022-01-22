using Pogo;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Logic
{
    public class DieListener : Listener
    {
        protected override void Listen()
        {
            PogoGameManager.PogoInstance?.OnPlayerDeath.AddListener(Heard);
        }


        protected override void StopListening()
        {
            PogoGameManager.PogoInstance?.OnPlayerDeath.RemoveListener(Heard);
        }
    }
}
