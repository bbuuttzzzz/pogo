using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Pogo.Cosmetics;
using System;
using static UnityEngine.Rendering.DebugUI;
using System.Globalization;
using System.IO;

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
            var directoryPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Source));
            var newCosmeticDescriptor = Instantiate(Source);
            var newEquipmentDescriptor = Instantiate(Source.Equipment);
            var newEquipmentPrefab = PrefabUtility.InstantiatePrefab(Source.Equipment.Prefab) as GameObject;


            newCosmeticDescriptor.name = NewFileName;
            newCosmeticDescriptor.DisplayName = NewCosmeticDisplayName;
            newCosmeticDescriptor.Equipment = newEquipmentDescriptor;

            newEquipmentDescriptor.name = NewEquipmentFileName;
            newEquipmentDescriptor.Prefab = newEquipmentPrefab;

            newEquipmentPrefab.name = NewFileName;

            if (!SpawnPrefabAsVariant)
            {
                PrefabUtility.UnpackPrefabInstance(newEquipmentPrefab, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
            }
            PrefabUtility.SaveAsPrefabAsset(newEquipmentPrefab, $"{directoryPath}{Path.DirectorySeparatorChar}{NewFileName}.prefab");

            AssetDatabase.CreateAsset(newEquipmentDescriptor, $"{directoryPath}{Path.DirectorySeparatorChar}{NewEquipmentFileName}.asset");
            AssetDatabase.CreateAsset(newCosmeticDescriptor, $"{directoryPath}{Path.DirectorySeparatorChar}{NewFileName}.asset");

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
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;

            NewCosmeticDisplayName = info.ToTitleCase(NewName);
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
