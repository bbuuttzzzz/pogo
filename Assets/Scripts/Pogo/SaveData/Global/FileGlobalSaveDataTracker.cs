using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Saving;
using Platforms;
using Assets.Scripts.Pogo.Difficulty;

namespace Pogo.Saving
{
    public class FileGlobalSaveDataTracker : GlobalSaveDataTracker
    {
        private const int CurrentSaveDataVersion = 0;
        private IPlatformService PlatformService;
        private string FilePath => $"{PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}{BaseName}.sav";
        private string BaseName => "global";

        public FileGlobalSaveDataTracker(IPlatformService platformService)
        {
            PlatformService = platformService;
        }

        public override void Save()
        {
            string rawDataSerialized = JsonConvert.SerializeObject(SaveData);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    writer.WriteLine(CurrentSaveDataVersion);
                    writer.WriteLine(rawDataSerialized);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to write {BaseName}.sav: {e}");
                return;
            }
        }

        public override void Load()
        {
            if (!File.Exists(FilePath))
            {
                DataState = DataStates.Empty;
                return;
            }

            string rawDataSerialized;
            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    // read off the CurrentSaveDataVersion this was written from
                    string versionLine = reader.ReadLine();
                    int version = int.Parse(versionLine);
                    if (version > CurrentSaveDataVersion)
                    {
                        throw new InvalidOperationException($"Incompatible save data of version {version} (expected {CurrentSaveDataVersion})");
                    }

                    rawDataSerialized = reader.ReadToEnd();
                }
            }

            catch (Exception e)
            {
                DataState = DataStates.Corrupt;
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to read {BaseName}.sav: {e}");
                return;
            }


            try
            {
                SaveData = JsonConvert.DeserializeObject<GlobalSaveData>(rawDataSerialized);
            }
            catch (Exception e)
            {
                DataState = DataStates.Corrupt;
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to deserialize {BaseName}.sav: {e}");
                return;
            }

            if (SaveData.collectibleUnlockDatas == null)
            {
                SaveData.collectibleUnlockDatas = new CollectibleUnlockData[0];
            }
            if (SaveData.challengeSaveDatas == null)
            {
                SaveData.challengeSaveDatas = new ChallengeSaveData[0];
            }

            DataState = DataStates.Loaded;
        }

        public override void InitializeNew()
        {
            SaveData = GlobalSaveData.NewData();
            DataState = DataStates.Loaded;
        }

        public override void Delete()
        {
            if (!File.Exists(FilePath)) return;
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"SaveSlot save ERROR Failed to delete {BaseName}.sav: {e}");
                return;
            }
        }
    }
}
