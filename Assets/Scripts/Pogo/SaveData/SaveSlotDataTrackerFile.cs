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

namespace Pogo.Saving
{
    public class SaveSlotDataTrackerFile : SaveSlotDataTracker
    {
        private IPlatformService platformService;
        private string FilePath => $"{platformService.PersistentDataPath}{Path.DirectorySeparatorChar}{BaseName}{Index}.sav";
        private string BaseName;
        private int Index;

        public SaveSlotDataTrackerFile(IPlatformService platformService, string baseName, int index)
        {
            BaseName = baseName;
            Index = index;
        }

        public override void Save()
        {
            string rawDataSerialized = JsonConvert.SerializeObject(RawData);
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
                RawData = SaveSlotData.NewGameData();
                return;
            }

            string rawDataSerialized;
            try
            {
                rawDataSerialized = File.ReadAllText(FilePath);
            }

            catch (Exception e)
            {
                RawData = SaveSlotData.NewGameData();
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to read {BaseName}.sav: {e}");
                return;
            }


            try
            {
                RawData = JsonConvert.DeserializeObject<SaveSlotData>(rawDataSerialized);
            }
            catch (Exception e)
            {
                RawData = SaveSlotData.NewGameData();
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to deserialize {BaseName}.sav: {e}");
            }

        }

    }
}
