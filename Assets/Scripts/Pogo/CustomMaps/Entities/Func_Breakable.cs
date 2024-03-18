using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Func_Breakable : WrappedEntityInstance
    {
        public const string Key_RegenOnPlayerSpawn = "resetondeath";

        public Func_Breakable(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("func_breakable", instance, context)
        {
        }

        public bool GetRegenOnPlayerSpawn() => Instance.entity.GetInt(Key_RegenOnPlayerSpawn) > 0;
    }
}
