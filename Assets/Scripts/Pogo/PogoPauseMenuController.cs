using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo
{
    public class PogoPauseMenuController : WizardUtils.PauseMenuController
    {
        GameSettingFloat FieldOfViewSetting;
        GameSettingFloat SensitivitySetting;

        protected override void Start()
        {
            base.Start();
            SensitivitySlider?.onValueChanged.AddListener(onSensitivitySliderChanged);
            FOVSlider?.onValueChanged.AddListener(onFOVChanged);
            FieldOfViewSetting = PogoGameManager.PogoInstance.FindGameSetting(PogoGameManager.KEY_FIELD_OF_VIEW);
            SensitivitySetting = PogoGameManager.PogoInstance.FindGameSetting(PogoGameManager.KEY_SENSITIVITY);
        }

        protected override void onPauseStateChanged(object sender, bool isPaused)
        {
            base.onPauseStateChanged(sender, isPaused);
            if (isPaused)
            {
                fov = FieldOfViewSetting.Value;
                FOVSlider.value = fov;
                sensitivity = SensitivitySetting.Value;
                SensitivitySlider.value = sensitivity * SENSITIVITY_SLIDER_CONVERSION;
            }
        }

        #region FOV

        float fov;
        float FOV
        {
            get => fov;
            set
            {
                FieldOfViewSetting.Value = value;
                fov = value;
            }
        }


        public Slider FOVSlider;

        private void onFOVChanged(float displayValue)
        {
            FOV = displayValue;
        }
        #endregion

        #region Sensitivity
        const float SENSITIVITY_SLIDER_CONVERSION = 100;

        const float SENSITIVITY_MINIMUM = 0.01f;
        const float SENSITIVITY_MAXIMUM = 2f;

        float sensitivity;
        float Sensitivity
        {
            get => sensitivity;
            set
            {
                sensitivity = Mathf.Clamp(value, SENSITIVITY_MINIMUM, SENSITIVITY_MAXIMUM);
                SensitivitySetting.Value = sensitivity;
            }
        }

        public Text SensitivityDisplayLabel;

        public Slider SensitivitySlider;

        private void onSensitivitySliderChanged(float displayValue)
        {
            Sensitivity = displayValue / SENSITIVITY_SLIDER_CONVERSION;

            SensitivityDisplayLabel.text = Sensitivity.ToString("N2");
        }
        #endregion
    }
}
