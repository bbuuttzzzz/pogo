using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Cosmetics
{
    [CustomEditor(typeof(VendingMachineManifest))]
    public class VendingMachineManifestEditor : Editor
    {
        private VendingMachineManifest self;

        private SerializedProperty m_Entries;
        List<int> misconfiguredCosmeticIndexes;

        public override VisualElement CreateInspectorGUI()
        {
            misconfiguredCosmeticIndexes = new List<int>();
            self = target as VendingMachineManifest;
            m_Entries = serializedObject.FindProperty(nameof(VendingMachineManifest.Entries));
            ValidateCosmetics();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Entries is automatically sorted. Editor is just bugged", MessageType.Info);
            EditorGUILayout.PropertyField(m_Entries);

            var oldArray = self.Entries.ToArray();

            serializedObject.ApplyModifiedProperties();

            if (!Enumerable.SequenceEqual(oldArray, self.Entries))
            {
                self.Sort();
                EditorUtility.SetDirty(self);
                ValidateCosmetics();
            }

            DrawValidateCosmeticsResult();
        }

        private void ValidateCosmetics()
        {
            misconfiguredCosmeticIndexes.Clear();
            for (int n = 0; n < self.Entries.Length; n++)
            {
                if (self.Entries[n].Cosmetic == null)
                {
                    continue;
                }
                if (self.Entries[n].Cosmetic.UnlockType != CosmeticDescriptor.UnlockTypes.VendingMachine)
                {
                    misconfiguredCosmeticIndexes.Add(n);
                }
            }
        }

        private void DrawValidateCosmeticsResult()
        {

            if (misconfiguredCosmeticIndexes.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The following Cosmetics are misconfigured:", MessageType.Error);
                using (new EditorGUI.DisabledScope(true))
                {
                    foreach (var index in misconfiguredCosmeticIndexes)
                    {
                        var label = new GUIContent(self.Entries[index].Cost.ToString());
                        _ = EditorGUILayout.ObjectField(label, self.Entries[index].Cosmetic, typeof(CosmeticDescriptor), false);
                    }
                }

                if (GUILayout.Button("Auto-Fix Now"))
                {
                    using (new UndoScope("Fix Misconfigured Cosmetics"))
                    {
                        foreach(var index in misconfiguredCosmeticIndexes)
                        {
                            var cosmetic = self.Entries[index].Cosmetic;
                            Undo.RecordObject(cosmetic, "");
                            cosmetic.UnlockType = CosmeticDescriptor.UnlockTypes.VendingMachine;
                            EditorUtility.SetDirty(cosmetic);
                            AssetDatabase.SaveAssetIfDirty(cosmetic);
                        }
                    }
                    ValidateCosmetics();
                }
            }
        }
    }

}
