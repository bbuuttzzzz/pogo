using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using WizardUtils;

namespace Pogo
{
    public class GlobalPostProcessController : MonoBehaviour
    {
        public Volume Volume;
        GameSettingFloat GammaSetting;
        DualRangeInterpolater interpolator;
        LiftGammaGain LiftGammaGainController;

        const float GammaValue_MinimumValue = -0.5f;
        const float GammaValue_MediumValue = 0;
        const float GammaValue_MaximumValue = 0.5f;

        private void Start()
        {
            interpolator = new DualRangeInterpolater(GammaValue_MinimumValue,
                                                     GammaValue_MediumValue,
                                                     GammaValue_MaximumValue);
            GammaSetting = GameManager.GameInstance.FindGameSetting(PogoGameManager.SETTINGKEY_GAMMA);
            GammaSetting.OnChanged += GammaSetting_OnChanged;
            SetGammaFromSetting(GammaSetting.Value);
        }

        private void GammaSetting_OnChanged(object sender, GameSettingChangedEventArgs e)
        {
            SetGammaFromSetting(e.FinalValue);
        }

        private void SetGammaFromSetting(float rawValue)
        {
            if (!Volume.profile.TryGet(out LiftGammaGainController))
            {
                Debug.LogError("Couldn't Find LiftGammaGain on GlobalPostProcess Volume");
                return;
            }


            float interpolatedValue = interpolator.Interpolate(rawValue / 100f);
            LiftGammaGainController.gamma.Override(new Vector4(1, 1, 1, interpolatedValue));
        }
    }
}
