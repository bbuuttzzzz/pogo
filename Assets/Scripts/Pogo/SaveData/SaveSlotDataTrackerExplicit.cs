using UnityEngine;

namespace Pogo.Saving
{
    internal class SaveSlotDataTrackerExplicit : SaveSlotDataTracker
    {
        private ExplicitSaveSlotData ExplicitSaveData;

        public SaveSlotDataTrackerExplicit(ExplicitSaveSlotData explicitSaveData)
        {
            ExplicitSaveData = explicitSaveData;
        }

        public override void Load()
        {
            RawData = ExplicitSaveData.Data;
        }

        public override void Save()
        {
            Debug.Log($"Pretended to save data to ExplicitSaveSlotData {ExplicitSaveData}");
        }
    }
}
