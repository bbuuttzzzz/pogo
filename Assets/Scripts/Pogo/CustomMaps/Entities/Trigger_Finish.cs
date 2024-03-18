using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Trigger_Finish : WrappedEntityInstance
    {
        public Trigger_Finish(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_finish", instance, context)
        {
        }
    }
}
