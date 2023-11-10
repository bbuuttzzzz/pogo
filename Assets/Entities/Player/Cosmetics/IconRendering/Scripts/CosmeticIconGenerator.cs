using Pogo.Cosmetics;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [RequireComponent(typeof(Camera))]
    public class CosmeticIconGenerator : MonoBehaviour
    {
        private const int IconWidth = 256;
        private const int IconHeight = 256;

        public bool RemapAlphaGradient;
        public AnimationCurve RemapAlphaCurve;

        [HideInInspector]
        public CosmeticDescriptor Cosmetic;
        public string DefaultPath;

        public Equipper equipper;

        public UnityEvent BeforeRender;
        public UnityEvent<CosmeticDescriptor> OnCosmeticChanged;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public Sprite RenderToSprite()
        {
            BeforeRender?.Invoke();
            Camera camera = GetComponent<Camera>();

            // create a new render texture
            RenderTexture renderTexture = new RenderTexture(IconWidth, IconHeight, 16, RenderTextureFormat.ARGB32);

            // render the camera to the renderTexture
            camera.targetTexture = renderTexture;
            camera.Render();

            Texture2D texture = GetRenderTargetPixels(renderTexture);

            if (RemapAlphaGradient)
            {
                for(int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        Color color = texture.GetPixel(x, y);
                        color.a = RemapAlphaCurve.Evaluate(color.a);
                        texture.SetPixel(x, y, color);
                    }
                }
            }

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
