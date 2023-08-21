using Platforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Saving
{
    public class SaveDataTrackerFile : SaveDataTracker
    {
        private IPlatformService PlatformService;

        public SaveDataTrackerFile(IPlatformService platformService, SaveManifest manifest) : base(manifest)
        {
            PlatformService = platformService;
        }

        public override void Save()
        {
            string path = Manifest.GetFilePath(PlatformService.PersistentDataPath);
            Debug.Log($"Writing to {path}");
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach ((_, SaveValue saveValue) in LoadedValues.ToList())
                {
                    if (saveValue.StringValue != saveValue.Descriptor.DefaultValue)
                    {
                        file.WriteLine($"{saveValue.Descriptor.Key} = {saveValue.StringValue}");
                    }
                }
            }
        }

        public override void Load()
        {
            LoadedValues = new Dictionary<SaveValueDescriptor, SaveValue>();
            string path = Manifest.GetFilePath(PlatformService.PersistentDataPath);

            if (!File.Exists(path)) return;
            using (StreamReader file = new StreamReader(path))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] results = line.Split('=', 2);
                    string key = results[0].Trim();
                    string value = results[1].Trim();

                    var descriptor = Manifest.FindByKey(key);
                    if (descriptor != null)
                    {
                        LoadedValues[descriptor] = new SaveValue(descriptor, value);
                    }
                    else
                    {
                        Debug.LogWarning($"Missing SaveData Descriptor for key \"{key}\"");
                    }
                }
            }   
        }
    }
}
