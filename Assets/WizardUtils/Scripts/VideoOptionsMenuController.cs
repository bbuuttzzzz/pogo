using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils
{
    public class VideoOptionsMenuController : MonoBehaviour
    {
        public bool VsyncEnabled { get; set; }
        public bool FullscreenEnabled { get; set; }

        public UnityEvent<bool> OnVsyncChanged;
        public UnityEvent<bool> OnFullscreenChanged;

        private void OnEnable()
        {
            Revert();
        }

        public void Revert()
        {
            VsyncEnabled = QualitySettings.vSyncCount > 0;
            FullscreenEnabled = Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
            OnVsyncChanged.Invoke(VsyncEnabled);
            OnFullscreenChanged.Invoke(FullscreenEnabled);
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
