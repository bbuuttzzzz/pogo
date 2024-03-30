using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using WizardUtils.Configurations;
using WizardUtils.Helpers;
using Pogo.CustomMaps.MapSources;
using LibBSP;

namespace Pogo.CustomMaps
{
    public static class MapHeaderHelper
    {
        public const string mapDefinitionFileName = "mapdefinition.txt";
        public const string previewSpriteFileName = "thumbnail.png";

        public static void SaveMapHeaderConfig(MapHeader header)
        {
            if (header.CfgPath == null)
            {
                header.CfgPath = $"{header.FolderPath}{Path.DirectorySeparatorChar}{mapDefinitionFileName}";
            }
            CfgFileSerializationHelper.Serialize(header.CfgPath, header.WriteCfgData());   
        }

        public static void SaveThumbnail(this MapHeader header, Sprite sprite)
        {
            string path = $"{header.FolderPath}{Path.DirectorySeparatorChar}{previewSpriteFileName}";

            File.WriteAllBytes(path, sprite.texture.EncodeToPNG());
            header.PreviewSprite = sprite;
            header.PreviewImagePath = path;
        }

        public static string GetMapName(string folderPath)
        {
            string bspPath = Directory.GetFiles(folderPath, "*.bsp")
                   .FirstOrDefault();

            if (bspPath != null)
            {
                return Path.GetFileNameWithoutExtension(bspPath);
            }

            return Path.GetFileName(folderPath);
        }

        public static GenerateMapHeaderResult GenerateMapHeader(MapLoadData loadData, bool logWarnings = true)
        {
            bool exit = false;
            MapHeader mapHeader = new MapHeader()
            {
                FolderPath = loadData.FolderPath
            };
            GenerateMapHeaderResult result = default;

            mapHeader.CfgPath = Directory.GetFiles(loadData.FolderPath, mapDefinitionFileName)
                .FirstOrDefault();

            if (!exit)
            {
                mapHeader.PreviewImagePath = Directory.GetFiles(loadData.FolderPath, previewSpriteFileName)
                    .FirstOrDefault();
            }

            if (!exit && mapHeader.CfgPath != null)
            {
                try
                {
                    var cfgdata = CfgFileSerializationHelper.Deserialize(mapHeader.CfgPath);
                    mapHeader.ReadCfgData(cfgdata);
                }
                catch(FormatException e)
                {
                    exit = true;
                    result = new GenerateMapHeaderResult(loadData, $"Failed to parse {mapDefinitionFileName}. {e.Message}");
                }
            }

            if (!exit)
            {
                string spritePath = $"{loadData.FolderPath}{Path.DirectorySeparatorChar}{previewSpriteFileName}";
                if (File.Exists(spritePath))
                {
                    try
                    {
                        mapHeader.PreviewSprite = ImageLoadingHelper.LoadSprite(spritePath);
                    }
                    catch(Exception e)
                    {
                        Debug.LogWarning($"Failed to parse {previewSpriteFileName} for map {mapHeader.FolderPath}. {e.Message}");
                    }
                }
            }

            if (!exit)
            {
                mapHeader.BspPath = Directory.GetFiles(loadData.FolderPath, "*.bsp")
                    .FirstOrDefault();

                if (mapHeader.BspPath == null)
                {
                    exit = true;
                    result = new GenerateMapHeaderResult(loadData, GenerateMapHeaderResult.FailReasons.MissingBSP);
                }
            }

            if (!exit)
            {
                mapHeader.MapName = Path.GetFileNameWithoutExtension(mapHeader.BspPath);
                result = new GenerateMapHeaderResult(loadData, mapHeader);
            }

            if (result.FailReason != GenerateMapHeaderResult.FailReasons.None
                && logWarnings)
            {
                result.LogWarnings();
            }
            return result;
        }
    }
}
