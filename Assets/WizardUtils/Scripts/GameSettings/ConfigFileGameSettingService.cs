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
        private bool IsDirty;
        private bool IsLoading;

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

        public GameSettingFloat GetSetting(string key)
        {
            return GameSettings[key];
        }
        public void Save()
        {
            if (!IsDirty) return;

            Tuple<string, float>[] data = new Tuple<string, float>[GameSettings.Count];
            int offset = 0;
            foreach (var kv in GameSettings)
            {
                data[offset++] = new Tuple<string, float>(kv.Key, kv.Value.Value);
            }

            string rawBlob;
            try
            {
                rawBlob = JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Settings save ERROR Failed to serialize {FileName}.cfg: {e}");
                return;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, rawBlob);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Settings save ERROR Failed to write {FileName}.cfg: {e}");
                return;
            }
        }
        private void RegisterGameSetting(GameSettingFloat newSetting)
        {
            GameSettings.Add(newSetting.Key, newSetting);
            newSetting.OnChanged += (sender, e) => OnGameSettingChanged(newSetting, e);
        }

        private void OnGameSettingChanged(GameSettingFloat setting, GameSettingChangedEventArgs e)
        {
            // we don't need to do any
            if (IsLoading) return;

            IsDirty = true;
        }

        private void Load()
        {
            IsLoading = true;
            Tuple<string, float>[] data;
            data = ReadData();

            foreach(var pair in data)
            {
                if (GameSettings.TryGetValue(pair.Item1, out var gameSetting))
                {
                    gameSetting.Value = pair.Item2;
                }
                else
                {
                    // register extra settings so they will get serialized when we re-save the config
                    RegisterGameSetting(new GameSettingFloat(pair.Item1, pair.Item2));
                }
            }
            IsLoading = false;
        }

        private Tuple<string, float>[] ReadData()
        {
            Tuple<string, float>[] data;
            string rawBlob;
            if (!File.Exists(FilePath))
            {
                IsDirty = true;
                return new Tuple<string, float>[0];
            }
            
            try
            {
                rawBlob = File.ReadAllText(FilePath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Settings load ERROR Failed to open {FileName}.cfg: {e}");
                IsDirty = true;
                return new Tuple<string, float>[0];
            }

            try
            {
                data = JsonConvert.DeserializeObject<Tuple<string, float>[]>(rawBlob);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Settings load ERROR Failed to deserialize {FileName}.cfg: {e}");
                return new Tuple<string, float>[0];
            }

            return data;
        }


    }
}
