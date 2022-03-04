using UnityEngine;
using UnityEngine.Audio;

namespace WizardUtils
{
    [RequireComponent(typeof(GameManager))]
    public class AudioManager : MonoBehaviour
    {
        public AudioMixer mixer;
        public AudioChannelSettingPair[] Channels;

        private void Start()
        {
            GameManager instance = GetComponent<GameManager>();
            foreach(var channel in Channels)
            {
                var setting = instance.FindGameSetting(channel.GameSettingKey);
                mixer.SetFloat(channel.MixerParamName, linearToLog(setting.Value));
                setting.OnChanged += (sender, e) => {
                    mixer.SetFloat(channel.MixerParamName, linearToLog(e.FinalValue));
                };
            }
        }

        private float linearToLog(float value)
        {
            return Mathf.Log10(value / 100f) * 20;
        }
    }

    [System.Serializable]
    public struct AudioChannelSettingPair
    {
        public string MixerParamName;
        public string GameSettingKey;
    }
}
