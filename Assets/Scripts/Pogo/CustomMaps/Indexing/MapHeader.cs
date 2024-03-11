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
        public ulong? WorkshopId;

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
                    case "WorkshopId":
                        WorkshopId = ulong.Parse(setting.Value);
                        break;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> WriteCfgData()
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrEmpty(AuthorName))
            {
                data.Add(new KeyValuePair<string, string>("Author", AuthorName));
            }
            if (!string.IsNullOrEmpty(Version))
            {
                data.Add(new KeyValuePair<string, string>("Version", Version));
            }
            if (WorkshopId.HasValue)
            {
                data.Add(new KeyValuePair<string, string>("WorkshopId", WorkshopId.Value.ToString()));
            }

            return data;
        }
    }
}
