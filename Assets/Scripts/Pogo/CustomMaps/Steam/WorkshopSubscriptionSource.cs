#if !DISABLESTEAMWORKS
using Pogo.CustomMaps.MapSources;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pogo.CustomMaps.Steam
{
    public class WorkshopSubscriptionSource : IMapSource
    {
        public bool AllowUpload => false;

        public IEnumerable<MapLoadData> GetMaps()
        {
            uint subscribedItemCount = SteamUGC.GetNumSubscribedItems();
            PublishedFileId_t[] files = new PublishedFileId_t[subscribedItemCount];
            subscribedItemCount = SteamUGC.GetSubscribedItems(files, subscribedItemCount);
            for (int n = 0; n < subscribedItemCount; n++)
            {
                if (!SteamUGC.GetItemInstallInfo(files[n], out ulong _, out string folderPath, 255, out uint _))
                {
                    Debug.LogWarning($"Failed to retrieve install info for workshop item ID {files[n]}. It might not be installed.");
                    continue;
                }

                yield return new MapLoadData()
                {
                    FolderPath = folderPath,
                    Source = "Workshop"
                };
            }
        }
    }
}
#endif