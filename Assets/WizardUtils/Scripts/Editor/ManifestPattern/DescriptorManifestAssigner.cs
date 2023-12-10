using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WizardUtils.ManifestPattern
{
    public class DescriptorManifestAssigner<TManifest, TDescriptor> where TManifest : ScriptableObject, IDescriptorManifest<TDescriptor> where TDescriptor : ScriptableObject
    {
        public void DrawRegisterDropdown(TDescriptor item, string dropdownLabel = "Set Manifests...")
        {
            if (EditorGUILayout.DropdownButton(new GUIContent(dropdownLabel), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                var manifests = AssetDatabase.FindAssets($"t:{typeof(TManifest).Name}")
                    .Select(id => AssetDatabase.LoadAssetAtPath<TManifest>(AssetDatabase.GUIDToAssetPath(id)))
                    .OrderBy(x => x.name);
                foreach (var manifest in manifests)
                {
                    bool containsItem = manifest.Contains(item);
                    menu.AddItem(new GUIContent(manifest.name), containsItem, () =>
                    {
                        if (containsItem)
                        {
                            manifest.Remove(item);
                        }
                        else
                        {
                            manifest.Add(item);
                        }
                        EditorUtility.SetDirty(manifest);
                        AssetDatabase.SaveAssetIfDirty(manifest);
                    });
                }
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }

        public void DrawRegisterButtons(ICollection<TDescriptor> items, string headerText = "Manifests")
        {
            GUILayout.Label(headerText, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var manifests = AssetDatabase.FindAssets($"t:{typeof(TManifest).Name}")
                .Select(id => AssetDatabase.LoadAssetAtPath<TManifest>(AssetDatabase.GUIDToAssetPath(id)))
                .OrderBy(x => x.name);
            foreach (var manifest in manifests)
            {
                bool containsNone = true;
                bool containsAll = true;
                foreach(var item in items)
                {
                    bool containsItem = manifest.Contains(item);
                    containsAll &= containsItem;
                    containsNone &= !containsItem;
                }

                bool toggle;
                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(manifest, typeof(ScriptableObject), false);
                    }

                    EditorGUI.showMixedValue = !(containsNone || containsAll);
                    toggle = EditorGUILayout.Toggle(containsAll);
                    EditorGUI.showMixedValue = false;
                }

                if (!toggle && containsAll)
                {
                    RemoveAll(items, manifest);
                }
                else if (toggle && !containsAll)
                {
                    AddAll(items, manifest);
                }
            }
            EditorGUI.indentLevel--;
        }

        private static void AddAll(ICollection<TDescriptor> items, TManifest manifest)
        {
            Undo.RecordObject(manifest, "Add Descriptors to Manifest");
            foreach (var item in items)
            {
                if (!manifest.Contains(item))
                {
                    manifest.Add(item);
                }
            }
            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssetIfDirty(manifest);
        }

        private static void RemoveAll(ICollection<TDescriptor> items, TManifest manifest)
        {
            Undo.RecordObject(manifest, "Remove Descriptors from Manifest");
            foreach (var item in items)
            {
                if (manifest.Contains(item))
                {
                    manifest.Remove(item);
                }
            }
            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssetIfDirty(manifest);
        }

        [Obsolete("use DrawRegisterButtons(ICollection<TDescriptor> item...")]
        public void DrawRegisterButtons(TDescriptor item, string headerText = "Manifests") => DrawRegisterButtons(new TDescriptor[] { item }, headerText);
    }
}
