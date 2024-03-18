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
        const string Key_RenderStyle = "renderstyle";

        public Trigger_Finish(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_finish", instance, context)
        {
        }

        public enum RenderStyles
        {
            Default = 0,
            Invisible = 1,
            UseMapTexture = 2
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
