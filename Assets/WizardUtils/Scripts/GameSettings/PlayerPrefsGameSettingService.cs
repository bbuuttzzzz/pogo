using System;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils.GameSettings
{
    public class PlayerPrefsGameSettingService : IGameSettingService
    {
        Dictionary<string, GameSettingFloat> GameSettings;

        public PlayerPrefsGameSettingService()
        {
            GameSettings = new Dictionary<string, GameSettingFloat>();
        }

        public void RegisterGameSetting(GameSettingFloat newSetting)
        {
            GameSettings.Add(newSetting.Key, newSetting);
            newSetting.OnChanged += (sender, e) => OnGameSettingChanged(newSetting, e);
        }

        private void OnGameSettingChanged(GameSettingFloat setting, GameSettingChangedEventArgs e)
        {
            PlayerPrefs.SetFloat(setting.Key, e.FinalValue);
        }
    }
}
