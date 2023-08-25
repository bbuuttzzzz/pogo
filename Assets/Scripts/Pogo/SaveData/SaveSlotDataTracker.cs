using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Saving
{
    public abstract class SaveSlotDataTracker
    {
        public SaveSlotData SlotData;
        public abstract void Save();
        public abstract void Load();
    }
}