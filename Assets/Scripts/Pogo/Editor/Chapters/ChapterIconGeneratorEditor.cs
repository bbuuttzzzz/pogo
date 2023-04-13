using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo
{
    [CustomEditor(typeof(ChapterIconGenerator))]
    public class ChapterIconGeneratorEditor : Editor
    {
        ChapterIconGenerator self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as ChapterIconGenerator;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (self.Chapter != null)
            {
                if (GUILayout.Button("Bake Icon"))
                {
                    BakeIcon();
                }

                if (self.Chapter.Icon != null)
                {
                    RenderIcon();
                }
                else
                {
                    EditorGUILayout.HelpBox("No Icon generated", MessageType.Warning);
                }
            }

            base.OnInspectorGUI();
        }

        private void RenderIcon()
        {
            // Get the texture from the sprite
            Texture2D texture = self.Chapter.Icon.texture;

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
            Undo.SetCurrentGroupName("Bake Level Icon");
            int group = Undo.GetCurrentGroup();

            var sprite = self.RenderToSprite();

            string assetPath;
            if (self.Chapter.Icon != null)
            {
                assetPath = AssetDatabase.GetAssetPath(self.Chapter.Icon);
                AssetDatabase.DeleteAsset(assetPath);
            }
            else
            {
                assetPath = AssetDatabase.GetAssetPath(self.Chapter);
                assetPath = $"{Path.GetDirectoryName(assetPath)}{Path.DirectorySeparatorChar}icon_{self.Chapter.name}.png";
            }
            File.WriteAllBytes(assetPath, sprite.texture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            var reimportedSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
            self.Chapter.Icon = reimportedSprite;
            Undo.RecordObject(self.Chapter, "Change Level Icon");
            EditorUtility.SetDirty(self.Chapter);

            Undo.CollapseUndoOperations(group);
        }
    }
}
