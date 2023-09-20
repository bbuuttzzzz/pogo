using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils
{
    public class VideoOptionsMenuController
    {
        public bool VsyncEnabled;
        public bool FullscreenEnabled;

        public void Revert()
        {
            VsyncEnabled = QualitySettings.vSyncCount > 0;
            FullscreenEnabled = Screen.fullScreen;
        }

        public void Apply()
        {
            QualitySettings.vSyncCount = VsyncEnabled ? 1 : 0;
            Screen.fullScreen = FullscreenEnabled;
        }
    }
}
