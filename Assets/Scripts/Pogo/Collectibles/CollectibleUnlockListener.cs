using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Collectibles
{
    public class CollectibleUnlockListener : MonoBehaviour
    {
        public CollectibleDescriptor Collectible;
        public bool CheckGlobalFile;

        public UnityEvent OnInitialized_UnCollected;
        public UnityEvent OnInitialized_Collected;
        public UnityEvent OnCollected;

        private void Start()
        {
            if ((CheckGlobalFile && CollectedInGlobalSave)
                || (!CheckGlobalFile && CollectedInSlotSave)
                )
            {
                OnInitialized_Collected?.Invoke();
            }
            else
            {
                OnInitialized_UnCollected?.Invoke();
                PogoGameManager.PogoInstance.OnCollectibleUnlocked.AddListener(Manager_OnCollectibleUnlocked);
            }
        }

        private void Manager_OnCollectibleUnlocked(CollectibleUnlockedEventArgs arg0)
        {
            if (arg0.Collectible != Collectible)
            {
                return;
            }

            OnCollected?.Invoke();
            PogoGameManager.PogoInstance.OnCollectibleUnlocked.RemoveListener(Manager_OnCollectibleUnlocked);
        }

        private bool CollectedInSlotSave
        {
            get
            {
                if (PogoGameManager.PogoInstance.CurrentSlotDataTracker == null)
                {
                    return false;
                }

                var data = PogoGameManager.PogoInstance.CurrentSlotDataTracker.GetCollectible(Collectible.Key);
                return data.isUnlocked;
            }
        }

        private bool CollectedInGlobalSave
        {
            get
            {
                if (PogoGameManager.PogoInstance.CurrentGlobalDataTracker == null)
                {
                    return false;
                }

                var data = PogoGameManager.PogoInstance.CurrentGlobalDataTracker.GetCollectible(Collectible.Key);
                return data.isUnlocked;
            }
        }
    }
}