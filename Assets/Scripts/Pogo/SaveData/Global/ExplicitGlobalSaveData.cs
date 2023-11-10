using Pogo.Collectibles;
using Pogo.Cosmetics;
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
        public CosmeticDescriptor[] equippedCosmetics;

        public GlobalSaveData Data
        {
            get
            {
                GlobalSaveData data = new GlobalSaveData()
                {
                    collectibleUnlockDatas = new CollectibleUnlockData[unlockedCollectibles.Length],
                    challengeSaveDatas = new ChallengeSaveData[0],
                    cosmeticEquipDatas = new CosmeticEquipData[equippedCosmetics.Length],
                };

                for (int n = 0; n < unlockedCollectibles.Length; n++)
                {
                    data.collectibleUnlockDatas[n] = new CollectibleUnlockData()
                    {
                        key = unlockedCollectibles[n].Key,
                        isUnlocked = true
                    };
                }

                for (int n = 0; n < equippedCosmetics.Length; n++)
                {
                    data.cosmeticEquipDatas[n] = new CosmeticEquipData()
                    {
                        Key = equippedCosmetics[n].Key,
                        Slot = equippedCosmetics[n].Slot,
                        ColorCode = "FFFFFF"
                    };
                }

                return data;
            }
        }
    }
}
