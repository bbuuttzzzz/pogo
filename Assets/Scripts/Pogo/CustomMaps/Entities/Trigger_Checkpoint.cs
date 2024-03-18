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
        const string Key_RenderStyle = "renderstyle";
        public enum RenderStyles
        {
            Default = 0,
            Invisible = 1,
            UseMapTexture = 2
        }

        public Trigger_Checkpoint(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_checkpoint", instance, context)
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

        public string GetOverrideSkipTargetName() => Instance.entity[Key_SkipTarget];
        public BSPLoader.EntityInstance GetSingleOverrideSkipTarget() => GetSingleTarget(Key_SkipTarget);

        public bool GetCanSkip() => GetSpawnFlag(1u);
        public RenderStyles GetRenderStyle()
        {
            int key = Instance.entity.GetInt(Key_RenderStyle, 0);
            if (key < 0 || key > (int)RenderStyles.UseMapTexture)
            {
                throw new FormatException($"{Instance.entity.ClassName} has bad RenderStyle {key}. expected 0, 1, or 2");
            }

            return (RenderStyles)key;
        }
    }
}
