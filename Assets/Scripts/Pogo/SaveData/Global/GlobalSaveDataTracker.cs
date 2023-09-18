using Pogo.Challenges;
using Pogo.Collectibles;
using Pogo.Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace Pogo.Saving
{
    public abstract class GlobalSaveDataTracker
    {
        public GlobalSaveData SaveData;
        public enum DataStates
        {
            Unloaded,
            Loaded,
            Empty,
            Corrupt
        }
        public DataStates DataState;

        public abstract void Save();

        public abstract void Load();
        public abstract void Delete();
        public abstract void InitializeNew();

        public int IndexOfCollectible(string collectibleId)
        {
            for (int n = 0; n < SaveData.collectibleUnlockDatas.Length; n++)
            {
                if (SaveData.collectibleUnlockDatas[n].key == collectibleId)
                {
                    return n;
                }
            }

            return -1;
        }
        public CollectibleUnlockData GetCollectible(string collectibleId)
        {
            int id = IndexOfCollectible(collectibleId);
            if (id == -1)
            {
                CollectibleUnlockData newCollectible = new CollectibleUnlockData()
                {
                    key = collectibleId,
                    isUnlocked = false
                };
                return newCollectible;
            }
            else
            {
                return SaveData.collectibleUnlockDatas[id];
            }
        }

        public void SetCollectible(CollectibleUnlockData data)
        {
            int id = IndexOfCollectible(data.key);
            if (id == -1)
            {
                ArrayHelper.InsertAndResize(ref SaveData.collectibleUnlockDatas, data);
            }
            else
            {
                SaveData.collectibleUnlockDatas[id] = data;
            }
        }

        public int IndexOfChallenge(string challengeId)
        {
            for (int n = 0; n < SaveData.challengeSaveDatas.Length; n++)
            {
                if (SaveData.challengeSaveDatas[n].key == challengeId)
                {
                    return n;
                }
            }

            return -1;
        }

        public ChallengeSaveData GetChallenge(string challengeId)
        {
            int id = IndexOfChallenge(challengeId);
            if (id == -1)
            {
                ChallengeSaveData newCollectible = new ChallengeSaveData()
                {
                    key = challengeId,
                    bestTimeMS = Challenge.WORST_TIME
                };
                return newCollectible;
            }
            else
            {
                return SaveData.challengeSaveDatas[id];
            }
        }

        public void SetChallenge(ChallengeSaveData data)
        {
            int id = IndexOfChallenge(data.key);
            if (id == -1)
            {
                ArrayHelper.InsertAndResize(ref SaveData.challengeSaveDatas, data);
            }
            else
            {
                SaveData.challengeSaveDatas[id] = data;
            }
        }


        public void UpdatePreviewData(CollectibleManifest collectibleManifest)
        {
            SaveData.CollectedCoins = CountCoins(collectibleManifest);
        }

        private int CountCoins(CollectibleManifest collectibleManifest)
        {
            int count = 0;
            foreach (var collectible in collectibleManifest.Collectibles)
            {
                if (collectible.CollectibleType != CollectibleDescriptor.CollectibleTypes.Coin)
                {
                    continue;
                }
                CollectibleUnlockData data = GetCollectible(collectible.Key);
                if (data.isUnlocked)
                {
                    count++;
                }
            }

            return count;
        }
    }
}