namespace WizardUtils.Audio
{
    [System.Serializable]
    public struct AudioChannelSettingPair
    {
        public string MixerParamName;
        public string GameSettingKey;

        public AudioChannelSettingPair(string mixerParamName, string gameSettingKey)
        {
            MixerParamName = mixerParamName;
            GameSettingKey = gameSettingKey;
        }
    }
}
