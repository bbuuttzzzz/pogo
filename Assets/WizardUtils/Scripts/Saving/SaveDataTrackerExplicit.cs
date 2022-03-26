using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils.Saving
{
    public class SaveDataTrackerExplicit : SaveDataTracker
    {
        ExplicitSaveData SaveData;

        public SaveDataTrackerExplicit(SaveManifest manifest, ExplicitSaveData saveData) : base(manifest)
        {
            SaveData = saveData;
        }

        public override void Load()
        {
            LoadedValues = new Dictionary<SaveValueDescriptor, SaveValue>();

            foreach (var value in SaveData.Data)
            {
                LoadedValues[value.Descriptor] = new SaveValue(value);
            }
        }

        public override void Save()
        {
            Debug.Log($"Pretended to save data to ExplicitSaveData {SaveData}");
        }

    }
}
