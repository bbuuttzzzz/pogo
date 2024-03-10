using BSPImporter;
using Pogo.Trains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Info_Track_Start : Info_Track
    {
        public Info_Track_Start(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("info_track_start", instance, context)
        {
        }
    }
}
