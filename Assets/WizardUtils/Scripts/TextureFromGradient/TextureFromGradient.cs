using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils.ShaderHelpers
{
    [CreateAssetMenu(fileName = "grad_", menuName = "WizardUtils/TextureFromGradient", order = 1)]
    public class TextureFromGradient : ScriptableObject
    {
        public int Width = 256;
        public int Height = 64;

        public Gradient Gradient;

        [HideInInspector]
        public Texture2D Texture;

        public Texture2D Generate()
        {
            var tempTexture = new Texture2D(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                Color color = Gradient.Evaluate(x / (float)Width);
                for (int y = 0; y < Height; y++)
                {
                    tempTexture.SetPixel(x, y, color);
                }
            }

            tempTexture.wrapMode = TextureWrapMode.Clamp;
            tempTexture.Apply();

            return tempTexture;
        }
    }
}
