using BSPImporter;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Trigger_Kill : WrappedEntityInstance
    {
        const string Key_KillType = "killtype";
        public Trigger_Kill(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_kill", instance, context)
        {
        }

        public int GetKillTypeId()
        {
            int key = GetIntOrDefault(Key_KillType, 1, 0);
            return key;
        }
    }
}
