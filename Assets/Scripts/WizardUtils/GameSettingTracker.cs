using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils
{
    public class GameSettingTracker : MonoBehaviour
    {
        public GameSettingChangedEvent OnSettingChanged;
        GameSettingFloat gameSetting;
        public string SettingName;

        private void Start()
        {
            gameSetting = GameManager.GameInstance.FindGameSetting(SettingName);
            gameSetting.OnChanged += onGameSettingChanged;
        }

        private void onGameSettingChanged(object sender, GameSettingChangedEventArgs e)
        {
            OnSettingChanged.Invoke(e.FinalValue);
        }
    }

    [System.Serializable]
    public class GameSettingChangedEvent : UnityEvent<float>
    {
    }
}