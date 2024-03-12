using BSPImporter.Materials;
using BSPImporter.Textures;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    public class PogoMaterialSource : IMaterialSource
    {
        public bool EnableLogging = true;
        private Material BaseMaterial;
        private Dictionary<string, IFillableShader> ShadersDictionary;
        private Dictionary<string, Material> StockMaterialDictionary;

        public PogoMaterialSource(
            Material baseMaterial,
            IEnumerable<IFillableShader> fillableShaders,
            IEnumerable<IStockMaterial> stockMaterials = null)
        {
            BaseMaterial = baseMaterial;
            ShadersDictionary = new Dictionary<string, IFillableShader>();
            foreach(var shader in fillableShaders)
            {
                ShadersDictionary.Add(shader.ShaderName, shader);
            }
            StockMaterialDictionary = new Dictionary<string, Material>();
            if (stockMaterials != null)
            {
                foreach (var stockMaterial in stockMaterials)
                {
                    RegisterMaterial(stockMaterial.Name, stockMaterial.Material);
                }
            }
        }

        public void RegisterMaterial(string name, Material material)
        {
            StockMaterialDictionary.Add(name, material);
        }

        public Material BuildMaterial(WadTextureData textureData)
        {
            if (!string.IsNullOrEmpty(textureData.Name)
                && StockMaterialDictionary.TryGetValue(textureData.Name, out Material material))
            {
                return material;
            }

            if (string.IsNullOrEmpty(textureData.ShaderName))
            {
                return BuildBaseMaterial(textureData);
            }

            if (ShadersDictionary.TryGetValue(textureData.ShaderName, out IFillableShader descriptor))
            {
                return BuildFillableShaderMaterial(textureData, descriptor);
            }

            if (EnableLogging)
            {
                Debug.LogWarning($"Couldn't find shader with name \'{textureData.ShaderName}\' for texture {textureData.Name}");
            }

            return BuildBaseMaterial(textureData);
        }

        private Material BuildFillableShaderMaterial(WadTextureData textureData, IFillableShader fillable)
        {
            Material newMaterial = new Material(fillable.BaseMaterial);

            if (fillable.ReplaceMainTex)
            {
                newMaterial.SetTexture("_MainTex", textureData.Texture);
            }

            foreach (var property in fillable.Properties)
            {
                if (textureData.Metadata.TryGetValue(property.PropertyName.ToLowerInvariant(), out string value))
                {
                    bool success = true;
                    switch (property.ParameterType)
                    {
                        case FillableShaderProperty.ParameterTypes.Float:
                            success = float.TryParse(value, out float valueFloat);
                            newMaterial.SetFloat(property.ParameterName, valueFloat);
                            break;
                        case FillableShaderProperty.ParameterTypes.Color:
                            success = ColorUtility.TryParseHtmlString(value, out Color valueColor);
                            newMaterial.SetColor(property.ParameterName, valueColor);
                            break;
                        case FillableShaderProperty.ParameterTypes.Vector2:
                            success = TryParseVector2(value, out Vector2 valueVec2);
                            newMaterial.SetVector(property.ParameterName, valueVec2);
                            break;
                        case FillableShaderProperty.ParameterTypes.Vector3:
                            success = TryParseVector3(value, out Vector3 valueVec3);
                            newMaterial.SetVector(property.ParameterName, valueVec3);
                            break;
                    }

                    if (!success && EnableLogging)
                    {
                        Debug.LogWarning($"Failed to parse FillableShader param {property.ParameterName} of expected type {property.ParameterType}. (got {value})");
                    }
                }
            }

            return newMaterial;
        }

        private static bool TryParseVector2(string value, out Vector2 result)
        {
            Regex parseRegex = new Regex("([0-9\\.]),\\s*([0-9\\.])", RegexOptions.Compiled);
            Match match = parseRegex.Match(value);
            if (!match.Success)
            {
                result = default;
                return false;
            }

            if (   !float.TryParse(match.Groups[1].Value, out float x)
                || !float.TryParse(match.Groups[2].Value, out float y))
            {
                result = default;
                return false;
            }

            result = new Vector2(x, y);
            result = new Vector2(x, y);
            return true;
        }

        private static bool TryParseVector3(string value, out Vector3 result)
        {
            Regex parseRegex = new Regex("([0-9\\.]),\\s*([0-9\\.]),\\s*([0-9\\.])", RegexOptions.Compiled);
            Match match = parseRegex.Match(value);
            if (!match.Success)
            {
                result = default;
                return false;
            }

            if (   !float.TryParse(match.Groups[1].Value, out float x)
                || !float.TryParse(match.Groups[2].Value, out float y)
                || !float.TryParse(match.Groups[3].Value, out float z))
            {
                result = default;
                return false;
            }

            result = new Vector3(x, y, z);
            return true;
        }

        private Material BuildBaseMaterial(WadTextureData textureData)
        {
            var material = new Material(BaseMaterial);
            material.mainTexture = textureData.Texture;
            material.name = textureData.Name;

            return material;
        }
    }
}
