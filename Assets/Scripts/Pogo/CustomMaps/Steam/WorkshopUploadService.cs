#if !DISABLESTEAMWORKS
using Platforms.Steam;
using Pogo;
using Pogo.CustomMaps.Indexing;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps.Steam
{
    public class WorkshopUploadService
    {
        private SteamPlatformService PlatformService;
        private PogoGameManager GameManager;
        public MapHeader CurrentMap;

        private APICall<CreateItemResult_t> CreateMap_Call;
        private APICall<SubmitItemUpdateResult_t> UpdateMap_Call;

        public WorkshopUploadService(SteamPlatformService platformService, PogoGameManager gameManager)
        {
            PlatformService = platformService;
            GameManager = gameManager;

            CreateMap_Call = new APICall<CreateItemResult_t>(nameof(CreateMap_Call));
            UpdateMap_Call = new APICall<SubmitItemUpdateResult_t>(nameof(UpdateMap_Call));
        }

        #region CreateMap


        public void CreateAndUpdateMap(MapHeader header, Action<UpdateMapResult> callback)
        {
            CurrentMap = header;
            SteamAPICall_t call = SteamUGC.CreateItem(SteamPlatformService.AppId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            CreateMap_Call.Set(call, (e) =>
            {
                var result = CreateMap_Process(e, header);
                if (!result.Success)
                {
                    callback(result);
                    return;
                }
                
                UpdateMap(header, callback);
            });
        }

        private UpdateMapResult CreateMap_Process(APICallResult<CreateItemResult_t> result, MapHeader header)
        {
            if (result.CallResult.m_eResult != EResult.k_EResultOK || result.IOFailure)
            {
                return new UpdateMapResult()
                {
                    Success = false,
                    ErrorMessage = $"There was a problem creating this workshop item ({result.CallResult.m_eResult})",
                    UpdatedHeader = null
                };
            }

            header.WorkshopId = (ulong)result.CallResult.m_nPublishedFileId;
            IndexingHelper.SaveMapHeaderConfig(header);

            return new UpdateMapResult()
            {
                Success = true,
                UpdatedHeader = header
            };
        }

        #endregion

        #region
        
        public void UpdateMap(MapHeader header, Action<UpdateMapResult> callback)
        {
            UGCUpdateHandle_t handle = SteamUGC.StartItemUpdate(SteamPlatformService.AppId, (PublishedFileId_t)header.WorkshopId);

            SteamUGC.SetItemTitle(handle, header.MapName);

            if (header.PreviewImagePath != null)
            {
                SteamUGC.SetItemPreview(handle, header.PreviewImagePath);
            }
            SteamUGC.SetItemContent(handle, header.FolderPath);

            SteamAPICall_t call = SteamUGC.SubmitItemUpdate(handle, null);
            UpdateMap_Call.Set(call, (e) =>
            {
                var result = UpdateMap_Process(e, header);
                callback(result);
            });
        }

        private UpdateMapResult UpdateMap_Process(APICallResult<SubmitItemUpdateResult_t> result, MapHeader header)
        {
            if (result.CallResult.m_eResult != EResult.k_EResultOK || result.IOFailure)
            {
                return new UpdateMapResult()
                {
                    Success = false,
                    ErrorMessage = $"There was a problem creating this workshop item ({result.CallResult.m_eResult})",
                    UpdatedHeader = null
                };
            }

            header.WorkshopId = (ulong)result.CallResult.m_nPublishedFileId;
            IndexingHelper.SaveMapHeaderConfig(header);

            return new UpdateMapResult()
            {
                Success = true,
                UpdatedHeader = header
            };
        }

        #endregion
    }
}
#endif