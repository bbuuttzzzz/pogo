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

        private APICall<CreateItemResult_t, CreateMapResult> CreateMap_Call;

        public WorkshopUploadService(SteamPlatformService platformService, PogoGameManager gameManager)
        {
            PlatformService = platformService;
            GameManager = gameManager;

            CreateMap_Call = new APICall<CreateItemResult_t, CreateMapResult>(CreateMap_Process);
        }

        #region CreateMap


        public void CreateMap(MapHeader header, Action<CreateMapResult> callback)
        {
            CurrentMap = header;
            SteamAPICall_t call = SteamUGC.CreateItem(SteamPlatformService.AppId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            CreateMap_Call.Set(call, callback);
        }

        private CreateMapResult CreateMap_Process(APICallResult<CreateItemResult_t> result)
        {
            if (result.CallResult.m_eResult != EResult.k_EResultOK || result.IOFailure)
            {
                return new CreateMapResult()
                {
                    Success = false,
                    ErrorMessage = $"There was a problem creating this workshop item ({result.CallResult.m_eResult})",
                    UpdatedHeader = null
            }
                    
            }
        }

        private void CreateMap_Callback(CreateItemResult_t param, bool bIOFailure)
        {
            if (CurrentMap == null || CreateMap_CachedCallback == null)
            {
                throw new InvalidOperationException($"Missing Cached Parameters for CreateMap API Call");
            }



        }

        #endregion
    }
}
#endif