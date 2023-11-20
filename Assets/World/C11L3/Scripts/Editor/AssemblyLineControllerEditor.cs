using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.AssemblyLines
{
    [CustomEditor(typeof(AssemblyLineController))]
    public class AssemblyLineControllerEditor : Editor
    {
        private AssemblyLineController self;

        public override VisualElement CreateInspectorGUI()
        {
            self = (AssemblyLineController)target;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Repopulate"))
            {
                Repopulate();
            }
        }

        private void Repopulate()
        {
            using var undoScope = new UndoScope("Repopulate AssemblyLineController");

            for(int n = self.transform.childCount - 1; n >= 0; n--)
            {
                Undo.DestroyObjectImmediate(self.transform.GetChild(n).gameObject);
            }

            float length = self.TargetAnimationClip.length;
            float roughCount = length / self.DelaySeconds;
            int goodCount = (int)roughCount;
            if (roughCount - goodCount > 0.01f)
            {
                Debug.LogWarning($"DelaySeconds isn't a nice multiple of TargetAnimation's duration! ~{(roughCount - goodCount) * self.DelaySeconds} seconds off");
            }

            for (int n = 0; n < goodCount; n++)
            {
                var newObject = (GameObject)PrefabUtility.InstantiatePrefab(self.Prefab, self.transform);

                var controller = newObject.GetComponent<AssemblyLineEntryController>();

                controller.CycleOffset = (self.DelaySeconds * n) / length;
                controller.SetStartingPosition();

                Undo.RegisterCreatedObjectUndo(newObject, "");
            }
        }

    }
}