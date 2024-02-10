using BSPImporter;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Trigger_Checkpoint : WrappedCreatedEntity
    {
        protected override string ClassName => "trigger_checkpoint";
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

        public Trigger_Checkpoint(BSPLoader.EntityCreatedCallbackData data) : base(data)
        {
        }

        public CheckpointId GetCheckpointId()
        {
            var id = new CheckpointId()
            {
                CheckpointType = (CheckpointTypes)Data.Instance.entity.GetInt(Key_CheckpointType, 0),
                CheckpointNumber = Data.Instance.entity.GetInt(Key_CheckpointNumber, 0)
            };

            if (id.CheckpointType != CheckpointTypes.MainPath && id.CheckpointType != CheckpointTypes.SidePath)
            {
                throw new FormatException($"{Data.Instance.entity.ClassName} with ID {id} has a bad pathtype. It should be either 0 (main) or 1 (side)");
            }

            return id;
        }

        public string GetOverrideSkipTargetName() => Data.Instance.entity[Key_SkipTarget];
        public BSPLoader.EntityInstance GetSingleOverrideSkipTarget() => GetSingleTarget(Key_SkipTarget);

        public bool GetCanSkip() => GetSpawnFlag(1u);
        public RenderStyles GetRenderStyle()
        {
            int key = Data.Instance.entity.GetInt(Key_RenderStyle, 0);
            if (key < 0 || key > (int)RenderStyles.UseMapTexture)
            {
                throw new FormatException($"{Data.Instance.entity.ClassName} has bad RenderStyle {key}. expected 0, 1, or 2");
            }

            return (RenderStyles)key;
        }
    }
}
