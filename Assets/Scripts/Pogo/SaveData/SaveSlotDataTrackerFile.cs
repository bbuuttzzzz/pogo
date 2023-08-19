using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Saving;

namespace Pogo.Saving
{
    public class SaveSlotDataTrackerFile : SaveSlotDataTracker
    {
        private string FilePath => $"{RootPath}/{BaseName}{Index}";
        private string RootPath;
        private string BaseName;
        private int Index;

        public SaveSlotDataTrackerFile(string rootPath, string baseName, int index)
        {
            RootPath = rootPath;
            BaseName = baseName;
            Index = index;
        }

        public override void Save()
        {
            string rawDataSerialized = JsonConvert.SerializeObject(RawData);
            File.WriteAllText(FilePath, rawDataSerialized);
        }

        public override void Load()
        {
            if (!File.Exists(FilePath))
            {
                RawData = SaveSlotData.NewGameData();
                return;
            }
            
            string rawDataSerialized = File.ReadAllText(FilePath);
            RawData = JsonConvert.DeserializeObject<SaveSlotData>(rawDataSerialized);
        }

    }
}
