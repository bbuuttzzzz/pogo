using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.CustomMaps.Indexing
{
    public class MapHeader
    {
        public string FolderPath;
        public string CfgPath;
        public string PreviewImagePath;
        public string BspPath;
        public string MapName;
        public Sprite PreviewSprite;

        public string Version;
        public string AuthorName;
        public string SkyTexture;
        public ulong? WorkshopId;
        public float? FogThickness;
        public Color? FogColor;

        public float FogThicknessOrDefault() => FogThickness ?? 0.01f;

        public Color FogColorOrDefault() => FogColor ?? Color.white;

        public void ReadCfgData(IEnumerable<KeyValuePair<string, string>> settings)
        {
            foreach(var setting in settings)
            {
                switch(setting.Key)
                {
                    case "Version":
                        Version = setting.Value;
                        break;
                    case "Author":
                        AuthorName = setting.Value;
                        break;
                    case "SkyTexture":
                        SkyTexture = setting.Value;
                        break;
                    case "WorkshopId":
                        WorkshopId = ulong.Parse(setting.Value);
                        break;
                    case "FogThickness":
                        FogThickness = float.Parse(setting.Value);
                        break;
                    case "FogColor":
                        if (!ColorUtility.TryParseHtmlString(setting.Value, out var color))
                        {
                            throw new FormatException($"FogColor '{setting.Value}' couldn't parse as a hex color. Expected format #RRGGBB");
                        }
                        FogColor = color;
                        break;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> WriteCfgData()
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();

            AddCfgDataIfNotEmpty(data, "Author", AuthorName);
            AddCfgDataIfNotEmpty(data, "Version", Version);
            AddCfgDataIfNotEmpty(data, "SkyTexture", SkyTexture);

            if (WorkshopId.HasValue)
            {
                data.Add(new KeyValuePair<string, string>("WorkshopId", WorkshopId.Value.ToString()));
            }
            if (FogThickness.HasValue)
            {
                data.Add(new KeyValuePair<string, string>("FogThickness", FogThickness.Value.ToString()));
            }
            if (FogColor.HasValue)
            {
                data.Add(new KeyValuePair<string, string>("FogColor", ColorUtility.ToHtmlStringRGB(FogColor.Value)));
            }

            return data;
        }

        private void AddCfgDataIfNotEmpty(List<KeyValuePair<string, string>> data, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                data.Add(new KeyValuePair<string, string>(key, value));
            }
        }
    }
}
