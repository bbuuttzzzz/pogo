using Assets.Scripts.Pogo.Difficulty;
using UnityEngine;

namespace Pogo.Saving
{
    public class ExplicitSaveSlotDataTracker : SaveSlotDataTracker
    {
        private ExplicitSaveSlotData ExplicitSaveData;

        public ExplicitSaveSlotDataTracker(ExplicitSaveSlotData explicitSaveData)
        {
            ExplicitSaveData = explicitSaveData;
        }

        public override void Load()
        {
            SlotData = ExplicitSaveData.Data;
            DataState = DataStates.Loaded;
        }

        public override void InitializeNew(string name, Difficulties difficulty)
        {
            throw new System.InvalidOperationException();
        }

        public override void Save()
        {
            Debug.Log($"Pretended to save data to ExplicitSaveSlotData {ExplicitSaveData}");
        }

        public override void Delete()
        {
            Debug.Log($"Pretended to delete data from ExplicitSaveSlotData {ExplicitSaveData}");
        }
    }
}
