using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Pickups
{
    public class PickupCollectedEventArgs
    {
        public PickupIds PickupId;

        public PickupCollectedEventArgs(PickupIds pickupId)
        {
            PickupId = pickupId;
        }
    }
}
