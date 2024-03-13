using UnityEngine;

namespace Pogo.CustomMaps.Indexing
{
    public struct GenerateMapHeaderResult
    {
        public FailReasons FailReason;
        public MapHeader Data;
        public string Path;
        public string CustomWarning;

        public enum FailReasons
        {
            None,
            Custom ,
            MissingMapDefinition,
            MissingBSP,
        }

        public readonly bool Success => FailReason == FailReasons.None;

        public GenerateMapHeaderResult(string path, FailReasons failReason)
        {
            Path = path;
            FailReason = failReason;
            Data = null;
            CustomWarning = null;
        }

        public GenerateMapHeaderResult(string path, MapHeader data)
        {
            Path = path;
            FailReason = FailReasons.None;
            Data = data;
            CustomWarning = null;
        }

        public GenerateMapHeaderResult(string path, string customWarning)
        {
            Path = path;
            FailReason = FailReasons.Custom;
            Data = null;
            CustomWarning = customWarning;
        }

        public void LogWarnings()
        {
            switch(FailReason)
            {
                case FailReasons.MissingMapDefinition:
                    Debug.LogWarning($"Couldn't find a mapdefinition.txt for map {Path} D:");
                    break;
                case FailReasons.MissingBSP:
                    Debug.LogWarning($"Couldn't find a .bsp for map {Path} D:");
                    break;
            }
        }
    }
}
