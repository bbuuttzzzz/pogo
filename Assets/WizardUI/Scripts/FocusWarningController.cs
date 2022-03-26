using Pogo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace WizardUtils
{
    public class FocusWarningController : ToggleableUIElement
    {
        public bool Debug;
        private void OnApplicationFocus(bool focus)
        {
#if UNITY_WEBGL
            SetOpen(GameManager.GameInstanceIsValid() && GameManager.GameInstance.InGameScene && !focus);
#else
            if (Debug) SetOpen(GameManager.GameInstanceIsValid() && GameManager.GameInstance.InGameScene && !focus);
#endif
        }
    }
}
