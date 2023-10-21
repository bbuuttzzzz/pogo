using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using WizardUtils.Audio;

namespace WizardUtils
{
    [RequireComponent(typeof(GameManager))]
    public class AudioManager : MonoBehaviour
    {
        private bool shouldMuteOnLoseFocus;
        private bool applicationHasFocus;
        private bool audioManagerSetUp = false;

        public AudioMixer mixer;
        public AudioChannelSettingPair[] AdditionalChannels;
        private static AudioChannelSettingPair[] BaseChannels => new AudioChannelSettingPair[]
        {
            new AudioChannelSettingPair("MasterVolume", GameManager.KEY_VOLUME_MASTER),
            new AudioChannelSettingPair("EffectVolume", GameManager.KEY_VOLUME_EFFECTS),
            new AudioChannelSettingPair("AmbienceVolume", GameManager.KEY_VOLUME_AMBIENCE),
            new AudioChannelSettingPair("MusicVolume", GameManager.KEY_VOLUME_MUSIC)
        };

        private AudioChannelController[] Controllers;

        private void Start()
        {
            GameManager instance = GetComponent<GameManager>();
            Controllers = new AudioChannelController[BaseChannels.Length];

            var allChannels = GenerateAllChannels();
            Controllers = new AudioChannelController[allChannels.Length];
            for (int n = 0; n < allChannels.Length; n++)
            {
                var setting = instance.FindGameSetting(allChannels[n].GameSettingKey);
                Controllers[n] = new AudioChannelController(mixer, setting, allChannels[n].MixerParamName);
            }

            GameSettingFloat altTabMuteSetting = GameManager.GameInstance.FindGameSetting(GameManager.SETTINGKEY_MUTE_ON_ALT_TAB);
            altTabMuteSetting.OnChanged += AltTabMuteSetting_OnChanged;
            shouldMuteOnLoseFocus = altTabMuteSetting.Value == 1;
            audioManagerSetUp = true;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!audioManagerSetUp) return;
            applicationHasFocus = focus;
            RecalculateShouldMute();
        }


        private void AltTabMuteSetting_OnChanged(object sender, GameSettingChangedEventArgs e)
        {
            shouldMuteOnLoseFocus = e.FinalValue == 1;
            RecalculateShouldMute();
        }

        private void RecalculateShouldMute()
        {
            SetGameMuted(shouldMuteOnLoseFocus && !applicationHasFocus);
        }

        public void SetGameMuted(bool muted)
        {
            Controllers[0].Muted = muted;
        }

        private AudioChannelSettingPair[] GenerateAllChannels() => BaseChannels.Concat(AdditionalChannels).ToArray();
    }
}
