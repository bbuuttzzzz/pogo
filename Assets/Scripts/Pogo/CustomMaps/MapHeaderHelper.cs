using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using WizardUtils.Configurations;
using WizardUtils.Helpers;

namespace Pogo.CustomMaps
{
    public static class MapHeaderHelper
    {
        public const string mapDefinitionFileName = "mapdefinition.txt";
        public const string previewSpriteFileName = "thumbnail.png";

        public static void SaveMapHeaderConfig(MapHeader header)
        {
            CfgFileSerializationHelper.Serialize(header.CfgPath, header.WriteCfgData());   
        }

        public static GenerateMapHeaderResult GenerateMapHeader(string folderPath, bool logWarnings = true)
        {
            bool exit = false;
            MapHeader mapHeader = new MapHeader()
            {
                FolderPath = folderPath
            };
            GenerateMapHeaderResult result = default;

            mapHeader.CfgPath = Directory.GetFiles(folderPath, mapDefinitionFileName)
                .FirstOrDefault();

            if (mapHeader.CfgPath == null)
            {
                exit = true;
                result = new GenerateMapHeaderResult(folderPath, GenerateMapHeaderResult.FailReasons.MissingMapDefinition);
            }

            if (!exit)
            {
                mapHeader.PreviewImagePath = Directory.GetFiles(folderPath, previewSpriteFileName)
                    .FirstOrDefault();
            }

            if (!exit)
            {
                try
                {
                    var cfgdata = CfgFileSerializationHelper.Deserialize(mapHeader.CfgPath);
                    mapHeader.ReadCfgData(cfgdata);
                }
                catch(FormatException e)
                {
                    exit = true;
                    result = new GenerateMapHeaderResult(folderPath, $"Failed to parse {mapDefinitionFileName} for map {mapHeader.FolderPath}. {e.Message}");
                }
            }

            if (!exit)
            {
                string spritePath = $"{folderPath}{Path.DirectorySeparatorChar}{previewSpriteFileName}";
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
                mapHeader.BspPath = Directory.GetFiles(folderPath, "*.bsp")
                    .FirstOrDefault();

                if (mapHeader.BspPath == null)
                {
                    exit = true;
                    result = new GenerateMapHeaderResult(folderPath, GenerateMapHeaderResult.FailReasons.MissingBSP);
                }
            }

            if (!exit)
            {
                mapHeader.MapName = Path.GetFileNameWithoutExtension(mapHeader.BspPath);
                result = new GenerateMapHeaderResult(folderPath, mapHeader);
            }
            
            result.LogWarnings();
            return result;
        }
    }
}
