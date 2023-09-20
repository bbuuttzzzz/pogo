using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils
{
    public class VideoOptionsMenuController : MonoBehaviour
    {
        public bool VsyncEnabled { get; set; }
        public bool FullscreenEnabled { get; set; }

        private void OnEnable()
        {
            Revert();
        }

        public void Revert()
        {
            VsyncEnabled = QualitySettings.vSyncCount > 0;
            FullscreenEnabled = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        }

        public void Apply()
        {
            QualitySettings.vSyncCount = VsyncEnabled ? 1 : 0;
            if (FullscreenEnabled)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }
}
