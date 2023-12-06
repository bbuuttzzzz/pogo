using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Pogo.Cosmetics;
using System;
using System.Globalization;
using System.IO;
using WizardUtils;

namespace Pogo.Tools
{
    public class CosmeticDescriptorCloneWizard : EditorWindow
    {
        private CosmeticDescriptor Source;
        private string OldName;
        private string newName;
        private string NewCosmeticDisplayName;
        private string NewEquipmentFileName => $"eq_{NewFileName}";
        private string NewFileName;

        private bool SpawnPrefabAsVariant;

        public string NewName
        {
            get => newName; set
            {
                newName = value;
                NewNameChanged();
            }
        }

        public static void Spawn(CosmeticDescriptor source)
        {
            var self = GetWindow<CosmeticDescriptorCloneWizard>();
            self.Source = source;

            self.OldName = GuessOldName(source.name);
            self.NewName = "";
            self.NewCosmeticDisplayName = "";
            self.NewFileName = "";
            self.SpawnPrefabAsVariant = false;
        }

        private void OnGUI()
        {
            Source = (CosmeticDescriptor) EditorGUILayout.ObjectField("Source", Source, typeof(CosmeticDescriptor), false);

            NewName = EditorGUILayout.TextField("New Name", NewName);
            SpawnPrefabAsVariant = EditorGUILayout.Toggle("As Variant", SpawnPrefabAsVariant);

            EditorGUILayout.Space();
            NewCosmeticDisplayName = EditorGUILayout.TextField("Display Name", NewCosmeticDisplayName);
            NewFileName = EditorGUILayout.TextField("File Name", NewFileName);
            EditorGUILayout.Space();
            
            if (CanClone())
            {
                if (GUILayout.Button("Clone"))
                {
                    Clone();
                }
            }
        }

        private void Clone()
        {
            using var undoScope = new UndoScope("Clone CosmeticDescriptor");
            var rootDirectoryPath = PathHelper.GetParentDirectory(Path.GetDirectoryName(AssetDatabase.GetAssetPath(Source)));
            var directoryPath = $"{rootDirectoryPath}{Path.DirectorySeparatorChar}{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NewName)}";
            Directory.CreateDirectory(directoryPath);

            // set up & writeEquipment Prefab
            string prefabPath = $"{directoryPath}{Path.DirectorySeparatorChar}{NewFileName}.prefab";
            var sceneObject = (GameObject)PrefabUtility.InstantiatePrefab(Source.Equipment.Prefab);
            if(!SpawnPrefabAsVariant)
            {
                PrefabUtility.UnpackPrefabInstance(sceneObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            }
            PrefabUtility.SaveAsPrefabAsset(sceneObject, prefabPath);

            var newEquipmentPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            newEquipmentPrefab.name = NewFileName;
            EditorUtility.SetDirty(newEquipmentPrefab);
            AssetDatabase.SaveAssetIfDirty(newEquipmentPrefab);

            // Clone Descriptors
            var newCosmeticDescriptor = Instantiate(Source);
            var newEquipmentDescriptor = Instantiate(Source.Equipment);

            // Tweak Values
            newCosmeticDescriptor.name = NewFileName;
            newCosmeticDescriptor.DisplayName = NewCosmeticDisplayName;
            newCosmeticDescriptor.Equipment = newEquipmentDescriptor;
            newCosmeticDescriptor.Icon = null;

            newEquipmentDescriptor.name = NewEquipmentFileName;
            newEquipmentDescriptor.Prefab = newEquipmentPrefab;

            // Create Files
            AssetDatabase.CreateAsset(newEquipmentDescriptor, $"{directoryPath}{Path.DirectorySeparatorChar}{NewEquipmentFileName}.asset");
            AssetDatabase.CreateAsset(newCosmeticDescriptor, $"{directoryPath}{Path.DirectorySeparatorChar}{NewFileName}.asset");

            Selection.activeObject = newCosmeticDescriptor;
        }

        private bool CanClone()
        {
            return Source != null
                && !string.IsNullOrWhiteSpace(NewCosmeticDisplayName)
                && !string.IsNullOrWhiteSpace(NewCosmeticDisplayName);
        }

        private void NewNameChanged()
        {
            if (string.IsNullOrWhiteSpace(OldName) || string.IsNullOrWhiteSpace(NewName)) return;

            NewCosmeticDisplayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NewName);
            NewFileName = Source.name.Replace(OldName, NewName);
        }




        private static string GuessOldName(string fullName)
        {
            int lastUnderscore = fullName.LastIndexOf('_');
            if (lastUnderscore == -1)
            {
                return fullName;
            }
            else
            {
                return fullName[(lastUnderscore+1)..];
            }
        }
    }
}
