using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Helpers
{
    public static class ImageLoadingHelper
    {
        public static Sprite LoadSprite(string path)
        {
            Texture2D texture = LoadTexture(path);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            return sprite;
        }

        public static Texture2D LoadTexture(string path)
        {
            byte[] fileData = System.IO.File.ReadAllBytes(path);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            return texture;
        }
    }
}
