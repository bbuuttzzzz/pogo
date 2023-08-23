using System;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils.GameSettings
{
    public class PlayerPrefsGameSettingService : IGameSettingService
    {
        Dictionary<string, GameSettingFloat> GameSettings;

        public PlayerPrefsGameSettingService(IEnumerable<GameSettingFloat> settings)
        {
            GameSettings = new Dictionary<string, GameSettingFloat>();
            foreach (GameSettingFloat newSetting in settings)
            {
                GameSettings.Add(newSetting.Key, newSetting);
                newSetting.OnChanged += (sender, e) => OnGameSettingChanged(newSetting, e);
            }
        }

        public GameSettingFloat GetSetting(string key)
        {
            return GameSettings[key];
        }

        public void Save()
        {
        }

        private void OnGameSettingChanged(GameSettingFloat setting, GameSettingChangedEventArgs e)
        {
            PlayerPrefs.SetFloat(setting.Key, e.FinalValue);
        }
    }
}
