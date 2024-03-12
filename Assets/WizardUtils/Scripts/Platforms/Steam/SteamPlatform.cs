#if !DISABLESTEAMWORKS
using Pogo;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.GameSettings;

namespace Platforms.Steam
{
    public class SteamPlatformService : IPlatformService
    {
        private bool initialized;
        public bool Initialized => initialized;
        public static AppId_t AppId => new(2413620u);
        private SteamAPIWarningMessageHook_t SteamAPIWarningMessageHook;
        private Callback<GameOverlayActivated_t> m_OnGameOverlayActivated;

        public string SaveDataPath { get; private set; }
        public string PersistentDataPath { get; private set; }

        public bool SupportsWorkshop => true;
        public string WorkshopLink => $"https://steamcommunity.com/app/{AppId}/workshop/";

        public string PlatformURLName => "steam";

        public SteamPlatformService()
        {
            try
            {
                if (SteamAPI.RestartAppIfNecessary(AppId))
                {
                    Debug.LogWarning("SteamApi RestartAppIfNecessary TRUE. Exiting App.");
                    Application.Quit();
                    return;
                }
            }
            catch (System.DllNotFoundException e)
            {
                Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. \n" + e);

                Application.Quit();
                return;
            }

            initialized = SteamAPI.Init();
            if (!initialized)
            {
                Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed.");
                Application.Quit();
            }

            SetupPersistentDataPath();
            PogoGameManager.PogoInstance.StartCoroutine(SpawnRunCallbacksCoroutine());
        }

        private IEnumerator SpawnRunCallbacksCoroutine()
        {
            while(initialized)
            {
                SteamAPI.RunCallbacks();
                yield return null;
            }
        }

        public IGameSettingService BuildGameSettingService(IEnumerable<GameSettingFloat> settings)
        {
            return new ConfigFileGameSettingService(this, "settings", settings);
        }

        #region Messages

        public void OnEnable()
        {
            if (!initialized)
            {
                return;
            }

            if (SteamAPIWarningMessageHook == null)
            {
                SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
                SteamClient.SetWarningMessageHook(SteamAPIWarningMessageHook);
            }

            m_OnGameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }

        public void OnDestroy()
        {
            if (!initialized)
            {
                return;
            }

            SteamAPI.Shutdown();
        }

        #endregion

        private void SetupPersistentDataPath()
        {
            AccountID_t steamId = SteamUser.GetSteamID().GetAccountID();

            PersistentDataPath = Application.persistentDataPath;
            // %home%/steamsaves/steam64id
            SaveDataPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}steamsaves{Path.DirectorySeparatorChar}{steamId}";
        }
        private void OnGameOverlayActivated(GameOverlayActivated_t callback)
        {
            if (callback.m_bActive != 0)
            {
                // overlay is active
                PogoGameManager.PogoInstance.Paused = true;
            }
            else
            {
                
            }
        }

        private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
        {
            Debug.LogWarning(pchDebugText);
        }

    }
}
#endif