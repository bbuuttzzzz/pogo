using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WizardUtils.Saving
{
    [CreateAssetMenu(fileName = "SaveManifest", menuName = "WizardUtils/Saving/SaveManifest", order = 1)]
    public class SaveManifest : ScriptableObject
    {
        public string FileName;
        public string GetFilePath(string basePath)
        {
            return $"{basePath}{Path.DirectorySeparatorChar}{FileName}.sav";
        }

        public SaveValueDescriptor[] SaveValueDescriptors;

        [ContextMenu("Clean Manifest")]
        public void CleanManifest()
        {
            List<SaveValueDescriptor> cleanedList = new List<SaveValueDescriptor>();

            foreach(var descriptor in SaveValueDescriptors)
            {
                if (descriptor == null) continue;
                cleanedList.Add(descriptor);
            }

            SaveValueDescriptors = cleanedList.ToArray();
        }

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
                if (otherDescriptor == null) continue;

                if (otherDescriptor.Key == key) return otherDescriptor;
            }

            return null;
        }
    }
}