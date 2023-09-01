using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Saving;
using Platforms;
using Assets.Scripts.Pogo.Difficulty;

namespace Pogo.Saving
{
    public class FileSaveSlotDataTracker : SaveSlotDataTracker
    {
        private const int CurrentSaveDataVersion = 0;
        private IPlatformService PlatformService;
        private string FilePath => $"{PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}{BaseName}{SaveSlotConstants.SaveSlotPath(slotId)}.sav";
        private string BaseName;
        private SaveSlotIds slotId;

        public FileSaveSlotDataTracker(IPlatformService platformService, string baseName, SaveSlotIds slotId)
        {
            PlatformService = platformService;
            BaseName = baseName;
            this.slotId = slotId;
        }

        public override void Save()
        {
            string rawDataSerialized = JsonConvert.SerializeObject(SlotData);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    writer.WriteLine(CurrentSaveDataVersion);
                    writer.WriteLine(rawDataSerialized);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to write {BaseName}.sav: {e}");
                return;
            }
        }

        public override void Load()
        {
            if (!File.Exists(FilePath))
            {
                DataState = DataStates.Empty;
                return;
            }

            string rawDataSerialized;
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    // read off the CurrentSaveDataVersion this was written from
                    _ = reader.ReadLine();
                    rawDataSerialized = reader.ReadToEnd();
                }
            }

            catch (Exception e)
            {
                DataState = DataStates.Corrupt;
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to read {BaseName}.sav: {e}");
                return;
            }


            try
            {
                SlotData = JsonConvert.DeserializeObject<SaveSlotData>(rawDataSerialized);
            }
            catch (Exception e)
            {
                DataState = DataStates.Corrupt;
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to deserialize {BaseName}.sav: {e}");
                return;
            }

            DataState = DataStates.Loaded;
        }

        public override void InitializeNew(string name, DifficultyId difficulty)
        {
            SlotData = SaveSlotData.NewGameData(name, difficulty);
            DataState = DataStates.Loaded;
        }

        public override void Delete()
        {
            if (!File.Exists(FilePath)) return;
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to delete {BaseName}.sav: {e}");
                return;
            }
        }
    }
}
