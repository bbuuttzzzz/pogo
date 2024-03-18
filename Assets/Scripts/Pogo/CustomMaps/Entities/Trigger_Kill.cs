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
        const string Key_RenderStyle = "renderstyle";
        const string Key_KillType = "killtype";
        public enum RenderStyles
        {
            Default = 0,
            Invisible = 1,
            UseMapTexture = 2
        }

        public Trigger_Kill(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_kill", instance, context)
        {
        }

        public int GetKillTypeId()
        {
            int key = Instance.entity.GetInt(Key_KillType, 0);
            return key;
        }

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
