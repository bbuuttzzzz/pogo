using Newtonsoft.Json;
using Platforms;
using Pogo.Saving;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WizardUtils.GameSettings
{
    public class ConfigFileGameSettingService : IGameSettingService
    {
        Dictionary<string, GameSettingFloat> GameSettings;
        private IPlatformService PlatformService;
        private string FilePath => $"{PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}{FileName}.cfg";
        private string FileName;


        public ConfigFileGameSettingService(IPlatformService platformService, string fileName, IEnumerable<GameSettingFloat> settings)
        {
            PlatformService = platformService;
            FileName = fileName;
            GameSettings = new Dictionary<string, GameSettingFloat>();
            foreach(var setting in settings)
            {
                RegisterGameSetting(setting);
            }
            Load();
        }

        private void RegisterGameSetting(GameSettingFloat newSetting)
        {
            GameSettings.Add(newSetting.Key, newSetting);
            newSetting.OnChanged += (sender, e) => OnGameSettingChanged(newSetting, e);
        }

        private void OnGameSettingChanged(GameSettingFloat setting, GameSettingChangedEventArgs e)
        {

        }

        private void Load()
        {
            Tuple<string, float>[] data;
            data = GetData();

            foreach(var pair in data)
            {
                if (GameSettings.TryGetValue(pair.Item1, out var gameSetting))
                {
                    gameSetting.Value = pair.Item2;
                    throw new NotImplementedException(":( this is gonna call OnGameSettingChanged a bunch?");
                }
            }
        }

        private Tuple<string, float>[] GetData()
        {
            Tuple<string, float>[] data;
            string rawBlob;
            if (!File.Exists(FilePath))
            {
                return new Tuple<string, float>[0];
            }
            
            try
            {
                rawBlob = File.ReadAllText(FilePath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Settings load ERROR Failed to open {FileName}.cfg: {e}");
                return new Tuple<string, float>[0];
            }

            try
            {
                data = JsonConvert.DeserializeObject<Tuple<string, float>[]>(rawBlob);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Settings load ERROR Failed to parse {FileName}.cfg: {e}");
                return new Tuple<string, float>[0];
            }

            return data;
        }
    }
}
