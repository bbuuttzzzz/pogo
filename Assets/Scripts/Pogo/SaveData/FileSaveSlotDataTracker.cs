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
                File.WriteAllText(FilePath, rawDataSerialized);
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
                DataLoaded = false;
                return;
            }

            string rawDataSerialized;
            try
            {
                rawDataSerialized = File.ReadAllText(FilePath);
            }

            catch (Exception e)
            {
                DataLoaded = false;
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to read {BaseName}.sav: {e}");
                return;
            }


            try
            {
                SlotData = JsonConvert.DeserializeObject<SaveSlotData>(rawDataSerialized);
            }
            catch (Exception e)
            {
                DataLoaded = false;
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to deserialize {BaseName}.sav: {e}");
            }

            DataLoaded = true;
        }

        public override void InitializeNew(string name, Difficulties difficulty)
        {
            SlotData = SaveSlotData.NewGameData(name, difficulty);
            DataLoaded = true;
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
