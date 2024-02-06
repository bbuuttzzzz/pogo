using BSPImporter;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Trigger_Checkpoint : WrappedEntity
    {
        protected override string ClassName => "trigger_checkpoint";
        const string Key_CheckpointNumber = "number";
        const string Key_CheckpointType = "pathtype";
        const string Key_SkipTarget = "skiptarget";

        public Trigger_Checkpoint(BSPLoader.EntityInstance instance) : base(instance)
        {
        }

        public CheckpointId GetCheckpointId()
        {
            var id = new CheckpointId()
            {
                CheckpointType = (CheckpointTypes)Instance.entity.GetInt(Key_CheckpointType, 0),
                CheckpointNumber = Instance.entity.GetInt(Key_CheckpointNumber, 0)
            };

            if (id.CheckpointType != CheckpointTypes.MainPath && id.CheckpointType != CheckpointTypes.SidePath)
            {
                throw new FormatException($"{Instance.entity.ClassName} with ID {id} has a bad pathtype. It should be either 0 (main) or 1 (side)");
            }

            return id;
        }

        public string GetOverrideSkipTarget() => Instance.entity[Key_SkipTarget];

        public bool GetCanSkip() => GetSpawnFlag(1u);
    }
}
