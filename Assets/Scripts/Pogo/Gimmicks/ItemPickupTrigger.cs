using Pogo.CustomMaps.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Pogo.Gimmicks
{
    public class ItemPickupTrigger : Trigger
    {
        public PickupIds PickupId;
        public UnityEvent OnRespawn;

        private void Awake()
        {
            OnActivated.AddListener(Base_OnActivated);
        }

        private void Base_OnActivated()
        {
            PogoGameManager.PogoInstance.OnPickupCollected?.Invoke(new PickupCollectedEventArgs(PickupId));
        }

        public void Respawn() => OnRespawn.Invoke();
    }
}
