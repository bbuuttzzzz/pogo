using UnityEngine;

namespace Pogo
{
    [RequireComponent(typeof(Camera))]
    public class ScreenshotMaker : MonoBehaviour
    {
        public int ImageWidth = 320;
        public int ImageHeight = 240;

        public Sprite Screenshot;
        public const string DefaultResultPath = "Assets/Screenshots";

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public Sprite RenderToSprite()
        {
            Camera camera = GetComponent<Camera>();

            // create a new render texture
            RenderTexture renderTexture = new RenderTexture(ImageWidth, ImageHeight, 16, RenderTextureFormat.ARGB32);

            // render the camera to the renderTexture
            camera.targetTexture = renderTexture;
            camera.Render();

            var texture = GetRenderTargetPixels(renderTexture);

            return Sprite.Create(texture,
                rect: new Rect(0, 0, ImageWidth, ImageHeight),
                pivot: new Vector2(0, 0));
        }

        private Texture2D GetRenderTargetPixels(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(ImageWidth, ImageHeight, TextureFormat.RGBA32, false);

            // Remember currently active render texture
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            texture.ReadPixels(new Rect(0, 0, ImageWidth, ImageHeight), 0, 0);
            texture.Apply();

            RenderTexture.active = currentActiveRT;
            return texture;
        }
    }
}
