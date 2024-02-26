using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Pogo.CustomMaps
{
    public struct MapSourceFolder
    {
        public string Path;
        public bool IsLocalPath;

        public MapSourceFolder(bool isLocalPath, string path)
        {
            IsLocalPath = isLocalPath;
            Path = path;
        }
    }
}
