using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Cosmetics
{
    [CustomEditor(typeof(CosmeticIconGenerator))]
    public class CosmeticIconGeneratorEditor : Editor
    {
        CosmeticIconGenerator self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CosmeticIconGenerator;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (self.Cosmetic != null)
            {
                if (GUILayout.Button("Bake Icon"))
                {
                    BakeIcon();
                }

                if (self.Cosmetic.Icon != null)
                {
                    RenderIcon();
                }
                else
                {
                    EditorGUILayout.HelpBox("No Icon generated", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select a Cosmetic", MessageType.Error);
            }

            base.OnInspectorGUI();
        }

        private void RenderIcon()
        {
            // Get the texture from the sprite
            Texture2D texture = self.Cosmetic.Icon.texture;

            // get a rect
            Rect rect = GUILayoutUtility.GetRect(texture.width, texture.height, GUILayout.ExpandWidth(false));

            // Center the rectangle horizontally
            float centerOffset = (EditorGUIUtility.currentViewWidth - texture.width) / 2f;
            rect.x += centerOffset;

            // Draw the texture in the preview rectangle
            EditorGUI.DrawPreviewTexture(rect, texture);
        }

        private void BakeIcon()
        {
            Undo.SetCurrentGroupName("Bake Icon");
            int group = Undo.GetCurrentGroup();

            var sprite = self.RenderToSprite();

            string assetPath;
            if (self.Cosmetic.Icon != null)
            {
                assetPath = AssetDatabase.GetAssetPath(self.Cosmetic.Icon);
                AssetDatabase.DeleteAsset(assetPath);
            }
            else
            {
                assetPath = AssetDatabase.GetAssetPath(self.Cosmetic);
                assetPath = $"{Path.GetDirectoryName(assetPath)}{Path.DirectorySeparatorChar}icon_{self.Cosmetic.name}.png";
            }
            File.WriteAllBytes(assetPath, sprite.texture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            var reimportedSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
            self.Cosmetic.Icon = reimportedSprite;
            Undo.RecordObject(self.Cosmetic, "Change Level Icon");
            EditorUtility.SetDirty(self.Cosmetic);

            Undo.CollapseUndoOperations(group);
        }
    }
}
