using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace WizardEffects
{
    [System.Serializable]
    public class EffectEvent : UnityEvent<EffectEventArgs>
    {
    }

    public class EffectEventArgs : EventArgs
    {
        public EffectData EffectData;

        public EffectEventArgs(EffectData effectData)
        {
            EffectData = effectData;
        }
    }
}
