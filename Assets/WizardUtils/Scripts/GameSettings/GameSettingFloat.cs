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
        private float _value;

        public EventHandler<GameSettingChangedEventArgs> OnChanged;

        public GameSettingFloat(string key, float defaultValue)
        {
            this.key = key;
            this.defaultValue = defaultValue;
            this._value = defaultValue;
        }

        public float Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    OnChanged?.Invoke(this, new GameSettingChangedEventArgs(_value, value));
                }
                _value = value;
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
