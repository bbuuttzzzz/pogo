using Pogo.CustomMaps.MapSources;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.CustomMaps.Indexing
{
    public struct GenerateMapHeaderResult
    {
        public FailReasons FailReason;
        public MapHeader Data;
        public MapLoadData LoadData;
        public string CustomWarning;

        public enum FailReasons
        {
            None,
            Custom ,
            MissingMapDefinition,
            MissingBSP,
        }

        public readonly bool Success => FailReason == FailReasons.None;

        public GenerateMapHeaderResult(MapLoadData loadData, FailReasons failReason)
        {
            LoadData = loadData;
            FailReason = failReason;
            Data = null;
            CustomWarning = null;
        }

        public GenerateMapHeaderResult(MapLoadData loadData, MapHeader data)
        {
            LoadData = loadData;
            FailReason = FailReasons.None;
            Data = data;
            CustomWarning = null;
        }

        public GenerateMapHeaderResult(MapLoadData loadData, string customWarning)
        {
            LoadData = loadData;
            FailReason = FailReasons.Custom;
            Data = null;
            CustomWarning = customWarning;
        }

        public void LogWarnings()
        {
            Debug.LogWarning(GetErrorText());
        }

        public string GetErrorText()
        {
            switch (FailReason)
            {
                case FailReasons.MissingMapDefinition:
                    return $"Couldn't find a mapdefinition.txt for map {LoadData} D:";
                case FailReasons.MissingBSP:
                    return $"Couldn't find a .bsp for map {LoadData} D:";
                case FailReasons.Custom:
                    return CustomWarning;
                default:
                    return $"Unknown fail reason {FailReason}";
            }
        }
    }
}
