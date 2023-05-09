using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WizardUtils.ShaderHelpers
{
    [CustomEditor(typeof(TextureFromGradient))]
    public class TextureFromGradientEditor : Editor
    {
        TextureFromGradient self;

        public override VisualElement CreateInspectorGUI()
        {
            self = (TextureFromGradient)target;
            VerifyAssetPath();

            return base.CreateInspectorGUI();
        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Bake"))
            {
                Bake();
            }

            if (self.Texture != null)
            {
                Render();
            }
            else
            {
                EditorGUILayout.HelpBox("No Generated Texture", MessageType.Warning);
            }
        }


        private void VerifyAssetPath()
        {
            if (self.Texture == null) return;

            string correctAssetPath;
            correctAssetPath = AssetDatabase.GetAssetPath(self);
            correctAssetPath = $"{Path.GetDirectoryName(correctAssetPath)}{Path.DirectorySeparatorChar}icon_{self.name}.png";

            string detectedAssetPath = AssetDatabase.GetAssetPath(self.Texture);

            if (detectedAssetPath != correctAssetPath)
            {
                self.Texture = null;
            }
        }

        private void Render()
        {
            // Get the texture from the sprite
            Texture2D texture = self.Texture;

            // get a rect
            Rect rect = GUILayoutUtility.GetRect(texture.width, texture.height, GUILayout.ExpandWidth(false));

            // Center the rectangle horizontally
            float centerOffset = (EditorGUIUtility.currentViewWidth - texture.width) / 2f;
            rect.x += centerOffset;

            // Draw the texture in the preview rectangle
            EditorGUI.DrawPreviewTexture(rect, texture);
        }

        private void Bake()
        {
            Undo.SetCurrentGroupName("Bake Gradient To Texture");
            int group = Undo.GetCurrentGroup();

            var texture = self.Generate();

            string assetPath;
            assetPath = AssetDatabase.GetAssetPath(self);
            assetPath = $"{Path.GetDirectoryName(assetPath)}{Path.DirectorySeparatorChar}icon_{self.name}.png";

            File.WriteAllBytes(assetPath, texture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            var reimportedSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
            self.Texture = reimportedSprite;
            Undo.RecordObject(self, "Bake Gradient To Texture");
            EditorUtility.SetDirty(self);

            Undo.CollapseUndoOperations(group);
        }
    }
}