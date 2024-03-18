using BSPImporter;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Trigger_Checkpoint : WrappedEntityInstance
    {
        const string Key_CheckpointNumber = "number";
        const string Key_CheckpointType = "pathtype";
        const string Key_SkipTarget = "skiptarget";

        public Trigger_Checkpoint(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_checkpoint", instance, context)
        {
        }

        public CheckpointId GetCheckpointId()
        {
            var id = new CheckpointId()
            {
                CheckpointType = (CheckpointTypes)GetIntOrDefault(Key_CheckpointType, 0, 0, 1),
                CheckpointNumber = GetIntOrDefault(Key_CheckpointNumber, 1, 0)
            };

            if (id.CheckpointType != CheckpointTypes.MainPath && id.CheckpointType != CheckpointTypes.SidePath)
            {
                throw new FormatException($"{ClassName} with ID {id} has a bad pathtype. It should be either 0 (main) or 1 (side)");
            }

            return id;
        }

        public string GetOverrideSkipTargetName() => GetValueOrDefault(Key_SkipTarget, "");
        public BSPLoader.EntityInstance GetSingleOverrideSkipTarget() => GetSingleTarget(Key_SkipTarget);

        public bool GetCanSkip() => GetSpawnFlag(1u);
    }
}
