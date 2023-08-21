using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils
{
    public class GameSettingFloat
    {
        string key;
        public string Key => key;
        float defaultValue;

        public EventHandler<GameSettingChangedEventArgs> OnChanged;

        public GameSettingFloat(string key, float defaultValue)
        {
            this.key = key;
            this.defaultValue = defaultValue;
        }

        public float Value
        {
            get => PlayerPrefs.GetFloat(key, defaultValue);
            set
            {
                float currentValue = Value;
                if (value != currentValue)
                {
                    OnChanged?.Invoke(this, new GameSettingChangedEventArgs(currentValue, value));
                }
            }
        }
    }

    public class GameSettingChangedEventArgs : EventArgs
    {
        public float InitialValue;
        public float FinalValue;

        public GameSettingChangedEventArgs(float initialValue, float finalValue)
        {
            InitialValue = initialValue;
            FinalValue = finalValue;
        }
    }
}
