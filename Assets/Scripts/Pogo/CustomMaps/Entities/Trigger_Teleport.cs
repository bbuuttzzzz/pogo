using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    internal class Trigger_Teleport : WrappedEntityInstance
    {
        const string Key_PreservePhysics = "keep_phys";
        const string Key_PreservePhysicsAngle = "phys_angle";

        public Trigger_Teleport(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_teleport", instance, context)
        {
        }

        public bool GetPreservePhysics() => SafeGetBool(Key_PreservePhysics);
        public float GetPreservePhysicsAngle() => SafeGetInt(Key_PreservePhysicsAngle, -360, 360);
    }
}
