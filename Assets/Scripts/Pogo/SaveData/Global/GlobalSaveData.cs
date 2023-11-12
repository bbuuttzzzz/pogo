using Pogo.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct GlobalSaveData
    {
        public CollectibleUnlockData[] collectibleUnlockDatas;
        public ChallengeSaveData[] challengeSaveDatas;
        public CosmeticEquipData[] cosmeticEquipDatas;

        public int CollectedCoins;
        public VendingMachineLastUnlockSaveData LastVendingMachineUnlock;


        public static GlobalSaveData NewData()
        {
            var data = new GlobalSaveData()
            {
                collectibleUnlockDatas = new CollectibleUnlockData[0],
                challengeSaveDatas = new ChallengeSaveData[0],
                cosmeticEquipDatas = new CosmeticEquipData[0],
            };

            return data;
        }
    }
}
