using BSPImporter;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Trigger_Kill : WrappedCreatedEntity
    {
        protected override string ClassName => "trigger_kill";
        const string Key_RenderStyle = "renderstyle";
        const string Key_KillType = "killtype";
        public enum RenderStyles
        {
            Default = 0,
            Invisible = 1,
            UseMapTexture = 2
        }

        public Trigger_Kill(BSPLoader.EntityCreatedCallbackData data) : base(data)
        {
        }

        public int GetKillTypeId()
        {
            int key = Data.Instance.entity.GetInt(Key_KillType, 0);
            return key;
        }

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
