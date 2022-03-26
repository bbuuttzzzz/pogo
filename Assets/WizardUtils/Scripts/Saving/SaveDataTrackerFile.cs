using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Saving
{
    public class SaveDataTrackerFile
    {
        public SaveManifest Manifest;
        public Dictionary<SaveValueDescriptor, SaveValue> LoadedValues;

        public SaveDataTrackerFile(SaveManifest manifest)
        {
            Manifest = manifest;
            LoadFromFile();
        }

        public string GetSaveValue(SaveValueDescriptor descriptor)
        {
            // check LoadedValues
            if (LoadedValues.TryGetValue(descriptor, out SaveValue value))
            {
                return value.StringValue;
            }

#if UNITY_EDITOR
            _ = ValidateDescriptor(descriptor);
#endif
            SaveValue newValue = AddFromDescriptor(descriptor);

            return newValue.StringValue;
        }

        private bool ValidateDescriptor(SaveValueDescriptor descriptor)
        {
            if (Manifest.ContainsDescriptor(descriptor))
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"Missing SaveValueDescriptor {descriptor} for manifest {Manifest}");
                return false;
            }
        }

        public void SetSaveValue(SaveValueDescriptor descriptor, string stringValue)
        {
            // check LoadedValues
            if (LoadedValues.TryGetValue(descriptor, out SaveValue value))
            {
                 value.StringValue = stringValue;
            }
        }

        private SaveValue AddFromDescriptor(SaveValueDescriptor descriptor)
        {
            var value = new SaveValue(descriptor);
            LoadedValues[descriptor] = value;

            return value;
        }

        public void SaveToFile()
        {
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

        private void LoadFromFile()
        {
            LoadedValues = new Dictionary<SaveValueDescriptor, SaveValue>();

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
