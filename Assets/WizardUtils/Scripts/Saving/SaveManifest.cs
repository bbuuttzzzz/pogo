using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils.Saving
{
    [CreateAssetMenu(fileName = "SaveManifest", menuName = "WizardUtils/Saving/SaveManifest", order = 1)]
    public class SaveManifest : ScriptableObject
    {
        public string FileName;
        public string DefaultPath => $"{Application.persistentDataPath}/{FileName}.sav";
        public SaveValueDescriptor[] SaveValueDescriptors;

        public bool ContainsDescriptor(SaveValueDescriptor descriptor)
        {
            foreach(SaveValueDescriptor otherDescriptor in SaveValueDescriptors)
            {
                if (descriptor == otherDescriptor) return true;
            }

            return false;
        }

        public SaveValueDescriptor FindByKey(string key)
        {
            foreach(SaveValueDescriptor otherDescriptor in SaveValueDescriptors)
            {
                if (otherDescriptor.Key == key) return otherDescriptor;
            }

            return null;
        }
    }
}