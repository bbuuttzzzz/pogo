using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils.ManifestPattern;

namespace Pogo.Collectibles
{
    public class CollectibleDescriptorManifestAssigner<TManifest, TDescriptor> where TManifest : ScriptableObject, IDescriptorManifest<TDescriptor> where TDescriptor : ScriptableObject
    {
        public UnityEvent OnNoSetManifests = new UnityEvent();

        public void DrawRegisterDropdown(TDescriptor item, string dropdownLabel = "Set Manifests...")
        {
            if (EditorGUILayout.DropdownButton(new GUIContent(dropdownLabel), FocusType.Passive))
            {
                bool found = false;

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
                            found = true;
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

                if (!found)
                {
                    OnNoSetManifests.Invoke();
                }
            }
        }

        public void DrawRegisterButtons(TDescriptor item, string headerText = "Manifests")
        {
            bool found = false;
            GUILayout.Label(headerText, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
            var manifests = AssetDatabase.FindAssets($"t:{typeof(TManifest).Name}")
                .Select(id => AssetDatabase.LoadAssetAtPath<TManifest>(AssetDatabase.GUIDToAssetPath(id)))
                .OrderBy(x => x.name);
            foreach (var manifest in manifests)
            {

                bool containsItem = manifest.Contains(item);

                bool toggle;
                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(manifest, typeof(ScriptableObject), false);
                    }
                    toggle = EditorGUILayout.Toggle(containsItem);
                }

                found = found || containsItem;
                if (!toggle && containsItem)
                {
                    manifest.Remove(item);
                    EditorUtility.SetDirty(manifest);
                    AssetDatabase.SaveAssetIfDirty(manifest);
                }
                else if (toggle && !containsItem)
                {
                    manifest.Add(item);
                    EditorUtility.SetDirty(manifest);
                    AssetDatabase.SaveAssetIfDirty(manifest);
                }
            }
            EditorGUI.indentLevel--;


            if (!found)
            {
                OnNoSetManifests.Invoke();
            }
        }
    }
}
