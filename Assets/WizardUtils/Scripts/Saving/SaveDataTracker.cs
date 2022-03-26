using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Saving
{
    public abstract class SaveDataTracker
    {
        public Dictionary<SaveValueDescriptor, SaveValue> LoadedValues;
        public SaveManifest Manifest;
        protected SaveDataTracker(SaveManifest manifest)
        {
            Manifest = manifest;
            Load();
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

        public void SetSaveValue(SaveValueDescriptor descriptor, string stringValue)
        {
            // check LoadedValues
            if (LoadedValues.TryGetValue(descriptor, out SaveValue value))
            {
                value.StringValue = stringValue;
            }
        }

        public abstract void Save();

        public abstract void Load();

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

        private SaveValue AddFromDescriptor(SaveValueDescriptor descriptor)
        {
            var value = new SaveValue(descriptor);
            LoadedValues[descriptor] = value;

            return value;
        }
    }
}
