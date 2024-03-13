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

        public MapSourceFolder(bool isLocalPath, string path)
        {
            AllowUpload = isLocalPath;
            Path = path;
            Directory.CreateDirectory(Path);
        }

        public IEnumerable<string> GetPaths()
        {
            return Directory.GetDirectories(Path);
        }
    }
}
