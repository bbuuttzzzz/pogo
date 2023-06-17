using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotMakerEditor : Editor
    {
        ScreenshotMaker self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as ScreenshotMaker;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
                if (GUILayout.Button("Bake Screenshot"))
                {
                    Bake();
                }

                if (self.Screenshot != null)
                {
                    Render();
                }
                else
                {
                    EditorGUILayout.HelpBox("No Icon generated", MessageType.Warning);
                }

            base.OnInspectorGUI();
        }

        private void Render()
        {
            // Get the texture from the sprite
            Texture2D texture = self.Screenshot.texture;

            float scale = EditorGUIUtility.currentViewWidth / texture.width;
            if (scale > 1) scale = 1;
            Vector2 renderedSize = new Vector2(scale * texture.width, scale * texture.height);

            // get a rect
            Rect rect = GUILayoutUtility.GetRect(renderedSize.x, renderedSize.y, GUILayout.ExpandWidth(false));

            // Center the rectangle horizontally
            float centerOffset = (EditorGUIUtility.currentViewWidth - renderedSize.x) / 2f;
            rect.x += centerOffset;

            // Draw the texture in the preview rectangle
            EditorGUI.DrawPreviewTexture(rect, texture);
        }

        private void Bake()
        {
            Undo.SetCurrentGroupName("Bake Level Icon");
            int group = Undo.GetCurrentGroup();

            var sprite = self.RenderToSprite();

            string assetPath;
            if (self.Screenshot != null)
            {
                assetPath = AssetDatabase.GetAssetPath(self.Screenshot);
                AssetDatabase.DeleteAsset(assetPath);
            }
            else
            {
                assetPath = ScreenshotMaker.DefaultResultPath;
                assetPath = $"{assetPath}{Path.DirectorySeparatorChar}Screenshot_{self.name}.png";
            }
            File.WriteAllBytes(assetPath, sprite.texture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            var reimportedSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
            self.Screenshot = reimportedSprite;
            Undo.RecordObject(self, "Regenerate Screenshot");
            EditorUtility.SetDirty(self);

            Undo.CollapseUndoOperations(group);
        }
    }
}
