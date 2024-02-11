using Pogo;
using Pogo.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor
{
    public static class WizardLitFixer
    {
        [MenuItem("Pogo/Fix WizardLit Textures")]

        public static void Run()
        {
            var wizardLitAsset = AssetDatabase.FindAssets("WizardLit")
                            .Select(id => AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(id)))
                .FirstOrDefault();

            if (wizardLitAsset == null)
            {
                throw new KeyNotFoundException();
            }

            foreach (var entry in new AssetEnumerator<Material>())
            {
                if (entry.shader == wizardLitAsset)
                {
                    var tex = entry.GetTexture("Texture2D_3344ACEE");
                    entry.SetTexture("_MainTex", tex);
                    EditorUtility.SetDirty(entry);
                    AssetDatabase.SaveAssetIfDirty(entry);
                }
            }
        }
    }
}
