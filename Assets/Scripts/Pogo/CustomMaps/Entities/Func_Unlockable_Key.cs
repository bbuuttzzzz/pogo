using BSPImporter;
using Pogo.Checkpoints;
using Pogo.CustomMaps.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Func_Unlockable_Key : WrappedEntityInstance
    {
        const string Key_KeyColor = "key_color";
        const string Key_AutoUnlockDelay = "auto_unlock";
        const PickupIds defaultKeyColor = PickupIds.RedKey;

        public Func_Unlockable_Key(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("func_unlockable_key", instance, context)
        {
        }

        public PickupIds GetKeyColor() => (PickupIds)GetIntOrDefault(Key_KeyColor, (int)defaultKeyColor, (int)PickupIds.RedKey, (int)PickupIds.YellowKey);

        public bool GetAutoUnlock() => GetIntOrDefault(Key_AutoUnlockDelay, 0) > 0;
        public float GetAutoUnlockDelaySeconds() => GetIntOrDefault(Key_AutoUnlockDelay, 0) == 1 ? 0.5f : 0f;
    }
}
