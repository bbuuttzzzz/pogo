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
        const PickupIds defaultKeyColor = PickupIds.RedKey;

        public Func_Unlockable_Key(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("func_unlockable_key", instance, context)
        {
        }

        public PickupIds GetKeyColor() => (PickupIds)GetIntOrDefault(Key_KeyColor, (int)defaultKeyColor, (int)PickupIds.RedKey, (int)PickupIds.YellowKey);
    }
}
