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
            Screen.fullScreenMode = FullscreenEnabled ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }
    }
}
