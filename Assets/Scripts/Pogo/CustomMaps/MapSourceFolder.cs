using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.MapSources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Pogo.CustomMaps
{
    public class MapSourceFolder : IMapSource
    {
        public string Path;
        public bool AllowUpload { get; private set; }
        private string SourceName;

        public MapSourceFolder(bool isLocalPath, string path, string sourceName)
        {
            AllowUpload = isLocalPath;
            Path = path;
            SourceName = sourceName;
            Directory.CreateDirectory(Path);
        }

        public IEnumerable<MapLoadData> GetMaps()
        {
            return Directory.GetDirectories(Path)
                .Select(path => GetLoadData(path));
        }

        private MapLoadData GetLoadData(string path)
        {
            return new MapLoadData()
            {
                FolderPath = path,
                Source = SourceName,
            };
        }
    }
}
