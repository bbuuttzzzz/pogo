using Pogo.Collectibles;
using Pogo.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Saving;

namespace Pogo.Saving
{
    [CreateAssetMenu(fileName = "ExplicitGlobalSaveData", menuName = "Pogo/Saving/ExplicitGlobalSaveData", order = 1)]

    public class ExplicitGlobalSaveData : ScriptableObject
    {
        public CollectibleDescriptor[] unlockedCollectibles;

        public GlobalSaveData Data
        {
            get
            {
                GlobalSaveData data = new GlobalSaveData()
                {
                    collectibleUnlockDatas = new CollectibleUnlockData[unlockedCollectibles.Length],
                    challengeSaveDatas = new ChallengeSaveData[0],
                };

                for (int n = 0; n < unlockedCollectibles.Length; n++)
                {
                    data.collectibleUnlockDatas[n] = new CollectibleUnlockData()
                    {
                        key = unlockedCollectibles[n].Key,
                        isUnlocked = true
                    };
                }

                return data;
            }
        }
    }
}
