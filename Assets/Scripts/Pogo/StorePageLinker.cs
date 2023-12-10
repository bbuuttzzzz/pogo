using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace Pogo
{
    public class StorePageLinker : MonoBehaviour
    {
        static string UnencodedUTMSource => $"app_{GameManager.GameInstance.PlatformService.PlatformURLName}_v{Application.version}";
        public string UnencodedUTMCampaign;

        public void OpenUrl()
        {
            string utmSource = Uri.EscapeDataString(UnencodedUTMSource);
            string utmCampaign = Uri.EscapeDataString(UnencodedUTMCampaign);
            string fullString = $"https://store.steampowered.com/app/2413620?utm_source={utmSource}&utm_campaign={utmCampaign}";
            Application.OpenURL(fullString);
        }
    }
}