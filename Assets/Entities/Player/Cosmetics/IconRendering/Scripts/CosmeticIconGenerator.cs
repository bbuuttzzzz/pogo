using UnityEngine;

namespace Pogo
{
    [RequireComponent(typeof(Camera))]
    public class CosmeticIconGenerator : MonoBehaviour
    {
        private const int IconWidth = 256;
        private const int IconHeight = 256;

        public Sprite Icon;
        public string DefaultPath;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public Sprite RenderToSprite()
        {
            Camera camera = GetComponent<Camera>();

            // create a new render texture
            RenderTexture renderTexture = new RenderTexture(IconWidth, IconHeight, 16, RenderTextureFormat.ARGB32);

            // render the camera to the renderTexture
            camera.targetTexture = renderTexture;
            camera.Render();

            var texture = GetRenderTargetPixels(renderTexture);

            return Sprite.Create(texture,
                rect: new Rect(0, 0, IconWidth, IconHeight),
                pivot: new Vector2(0, 0));
        }

        private Texture2D GetRenderTargetPixels(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(IconWidth, IconHeight, TextureFormat.RGBA32, false);

            // Remember currently active render texture
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = renderTexture;

            texture.ReadPixels(new Rect(0, 0, IconWidth, IconHeight), 0, 0);
            texture.Apply();

            RenderTexture.active = currentActiveRT;
            return texture;
        }
    }
}
