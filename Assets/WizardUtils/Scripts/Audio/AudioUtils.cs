using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Audio
{
    public static class AudioUtils
    {

        public static float LinearAudioLevelToLog(float value)
        {
            return Mathf.Log10(value / 100f) * 20;
        }
    }
}
