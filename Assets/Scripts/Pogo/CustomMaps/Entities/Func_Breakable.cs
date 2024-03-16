using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Func_Breakable : WrappedCreatedEntity
    {
        public const string Key_RegenOnPlayerSpawn = "resetondeath";

        public Func_Breakable(BSPLoader.EntityCreatedCallbackData data) : base("func_breakable", data)
        {
        }

        public bool GetRegenOnPlayerSpawn() => Data.Instance.entity.GetInt(Key_RegenOnPlayerSpawn) > 0;
    }
}
