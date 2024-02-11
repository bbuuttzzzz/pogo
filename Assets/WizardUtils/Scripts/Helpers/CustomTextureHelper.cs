using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils
{
    public static class CustomTextureHelper
    {
        public static string CustomFolderPath => $"{GameManager.GameInstance.SaveDataPath}{Path.DirectorySeparatorChar}custom";
        public static Texture2D Load(string name, Texture2D fallbackTexture)
        {
            Directory.CreateDirectory(CustomFolderPath);
            string filePath = $"{CustomFolderPath}{Path.DirectorySeparatorChar}{name}";

            if (!File.Exists(filePath))
            {
                Debug.Log($"File not found for custom Texture @ {filePath}. using default");
                return fallbackTexture;
            }

            byte[] rawFile = File.ReadAllBytes(filePath);
            Texture2D loadedTexture = new Texture2D(fallbackTexture.width, fallbackTexture.height);
            if (!ImageConversion.LoadImage(loadedTexture, rawFile))
            {
                Debug.Log($"Failed to load custom Texture @ {filePath}. using default");
                return fallbackTexture;
            }

            return loadedTexture;
        }
    }
}
