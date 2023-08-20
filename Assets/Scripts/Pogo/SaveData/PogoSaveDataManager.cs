using System;
using UnityEngine;

namespace Pogo.Saving
{
    public class PogoSaveDataManager : MonoBehaviour
    {
        private SaveSlotDataTracker CurrentTracker;

        public bool TrackerLoaded => CurrentTracker != null;

        public void LoadSlot(int slotId)
        {
            if (TrackerLoaded) throw new InvalidOperationException("Tried to load a save slot while one is already loaded");
        }

        public void SaveCurrentSlot(bool closeAfterSaving = false)
        {

        }
    }
}
