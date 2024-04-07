using BSPImporter;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Func_Unlockable_Coin : WrappedEntityInstance
    {
        const string Key_CoinsRequired = "coins_required";
        const string Key_AutoUnlockDelay = "auto_unlock_delay";
        const uint Flag_AutoUnlock = 1;

        public Func_Unlockable_Coin(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("func_unlockable_coin", instance, context)
        {
        }

        public int GetCoinsRequired() => GetIntOrDefault(Key_CoinsRequired, 1);
        public bool GetAutoUnlock() => GetSpawnFlag(Flag_AutoUnlock);
        public float GetAutoUnlockDelaySeconds()
        {
            int delayMilliseconds = GetIntOrDefault(Key_AutoUnlockDelay, 500);
            return delayMilliseconds / 1000f;
        }
    }
}
