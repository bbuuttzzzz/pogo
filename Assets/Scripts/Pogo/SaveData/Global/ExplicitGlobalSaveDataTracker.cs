using Assets.Scripts.Pogo.Difficulty;
using UnityEngine;
using WizardUtils.Saving;

namespace Pogo.Saving
{
    public class ExplicitGlobalSaveDataTracker : GlobalSaveDataTracker
    {
        private ExplicitGlobalSaveData ExplicitSaveData;

        public ExplicitGlobalSaveDataTracker(ExplicitGlobalSaveData explicitSaveData)
        {
            ExplicitSaveData = explicitSaveData;
        }

        public override void Load(bool createIfEmpty = false)
        {
            SaveData = ExplicitSaveData.Data;
            DataState = DataStates.Loaded;
        }

        public override void InitializeNew()
        {
            throw new System.InvalidOperationException();
        }

        public override void Save()
        {
            Debug.Log($"Pretended to save data to ExplicitGlobalSaveData {ExplicitSaveData}");
        }

        public override void Delete()
        {
            Debug.Log($"Pretended to delete data from ExplicitGlobalSaveData {ExplicitSaveData}");
        }
    }
}
