using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Pogo.Tools
{
    public class LightingBakeGroupSelectWizard : EditorWindow
    {
        public LightingBakeGroup SelectedBakeGroup;

        public static void Spawn()
        {
            _ = GetWindow<LightingBakeGroupSelectWizard>();
        }

        private void OnGUI()
        {
            SelectedBakeGroup = (LightingBakeGroup)EditorGUILayout.ObjectField("Bake Group", SelectedBakeGroup, typeof(LightingBakeGroup), false);

            if (SelectedBakeGroup != null)
            {
                if (GUILayout.Button("Open"))
                {
                    LightingBakeGroupLoader.OpenLightingBakeGroup(SelectedBakeGroup);
                }
            }
        }
    }
}
