using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace WizardUI
{
    public class GameSettingChanger : MonoBehaviour
    {
        public string SettingKeyName;
        GameSettingFloat setting;
        public UnityFloatEvent OnValueLoaded;

        private void Start()
        {
            setting = GameManager.GameInstance.FindGameSetting(SettingKeyName);
            OnValueLoaded?.Invoke(setting.Value);
        }

        public void SetValue(float newValue)
        {
            setting.Value = newValue;
        }
    }
}