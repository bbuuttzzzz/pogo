using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Collectibles
{
    [CustomEditor(typeof(CollectibleManifest))]
    public class CollectibleManifestEditor : Editor
    {
        CollectibleManifest self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CollectibleManifest;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.LabelField("Collectible Type Counts");
            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var group in self.Collectibles.GroupBy(c => c.CollectibleType).OrderBy(x => x.Key))
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.IntField(group.Key.ToString(), group.Count());
                    }
                }
            }
        }

    }
}