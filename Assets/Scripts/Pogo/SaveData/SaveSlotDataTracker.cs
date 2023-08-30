using Assets.Scripts.Pogo.Difficulty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Saving
{
    public abstract class SaveSlotDataTracker
    {
        public SaveSlotData SlotData;
        public bool DataLoaded;
        public abstract void Save();
        public abstract void Load();
        public abstract void Delete();
        public abstract void InitializeNew(string name, Difficulties difficulty);

    }
}