using System.Collections.Generic;
using UnityEngine;

namespace Pogo.CustomMaps.Indexing
{
    public class MapHeader
    {
        public string FolderPath;
        public string Version;
        public string AuthorName;
        public string CfgPath;
        public string BspPath;
        public string MapName;
        public Sprite PreviewSprite;

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
                }
            }
        }
    }
}
