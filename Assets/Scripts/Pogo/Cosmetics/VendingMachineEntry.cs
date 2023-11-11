using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Cosmetics
{
    [System.Serializable]
    public struct VendingMachineEntry
    {
        public int UnlockThreshold;
        public CosmeticDescriptor Cosmetic;
    }
}
