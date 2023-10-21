using Pogo.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WizardUtils.BakingTools
{
    public static class BakeModeMenuScripts
    {
        [MenuItem("Pogo/Baking/Enter Bake Mode")]
        public static void EnterBakeMode()
        {
            using (new UndoScope("Enter Bake Mode"))
            {
                var bakingObjects = UnityEngine.Object.FindObjectsOfType<BakingObject>(true);
                foreach (BakingObject obj in bakingObjects)
                {
                    Undo.RecordObject(obj, "");
                    obj.gameObject.SetActive(obj.EnabledWhenBaking);
                    EditorUtility.SetDirty(obj.gameObject);
                }
            }
        }

        [MenuItem("Pogo/Baking/Exit Bake Mode")]
        public static void ExitBakeMode()
        {
            using (new UndoScope("Exit Bake Mode"))
            {
                var bakingObjects = UnityEngine.Object.FindObjectsOfType<BakingObject>(true);
                foreach (BakingObject obj in bakingObjects)
                {
                    Undo.RecordObject(obj, "");
                    obj.gameObject.SetActive(obj.EnabledWhenNotBaking);
                    EditorUtility.SetDirty(obj.gameObject);
                }
            }
        }

        private static string[] GetScenes()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }
    }
}
