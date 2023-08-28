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
            DataLoaded = true;
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
