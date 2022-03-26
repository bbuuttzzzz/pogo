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
        public SaveDataTrackerFile(SaveManifest manifest) : base(manifest)
        {
        }

        public override void Save()
        {
            Debug.Log($"Writing to {Manifest.DefaultPath}");
            using (StreamWriter file = new StreamWriter(Manifest.DefaultPath))
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

            if (!File.Exists(Manifest.DefaultPath)) return;
            using (StreamReader file = new StreamReader(Manifest.DefaultPath))
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
