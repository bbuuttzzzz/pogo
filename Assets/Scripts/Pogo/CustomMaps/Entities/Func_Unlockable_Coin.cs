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

        public Func_Unlockable_Coin(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("func_unlockable_coin", instance, context)
        {
        }

        public int GetCoinsRequired() => GetIntOrDefault(Key_CoinsRequired, 1);
    }
}
