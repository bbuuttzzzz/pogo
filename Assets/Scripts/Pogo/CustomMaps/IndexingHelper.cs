using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WizardUtils.Configurations;

namespace Pogo.CustomMaps
{
    public static class IndexingHelper
    {
        public static GenerateMapHeaderResult GenerateMapHeader(string folderPath, bool logWarnings = true)
        {
            bool exit = false;
            MapHeader mapHeader = new MapHeader()
            {
                FolderPath = folderPath
            };
            GenerateMapHeaderResult result = default;

            mapHeader.CfgPath = Directory.GetFiles(folderPath, "*.cfg")
                .FirstOrDefault();

            if (mapHeader.CfgPath == null)
            {
                exit = true;
                result = new GenerateMapHeaderResult(folderPath, GenerateMapHeaderResult.FailReasons.MissingCfg);
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
                    result = new GenerateMapHeaderResult(folderPath, $"Failed to parse .cfg for map {mapHeader}. {e.Message}");
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
                mapHeader.MapName = Path.GetFileNameWithoutExtension(mapHeader.CfgPath);
                result = new GenerateMapHeaderResult(folderPath, mapHeader);
            }
            
            result.LogWarnings();
            return result;
        }
    }
}
