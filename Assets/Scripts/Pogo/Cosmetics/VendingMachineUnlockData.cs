using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Cosmetics
{
    [System.Serializable]
    public struct VendingMachineUnlockData
    {
        public int CoinsNeeded;
        public CosmeticDescriptor Cosmetic;

        public readonly bool RewardAvailable => CoinsNeeded <= 0;
    }
}
