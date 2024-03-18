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
        const string Key_RenderStyle = "renderstyle";
        public enum RenderStyles
        {
            Default = 0,
            Invisible = 1,
            UseMapTexture = 2
        }

        public Trigger_Teleport(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("trigger_teleport", instance, context)
        {
        }

        public bool GetPreservePhysics() => Instance.entity.GetInt(Key_PreservePhysics) > 0;
        public float GetPreservePhysicsAngle() => Instance.entity.GetInt(Key_PreservePhysicsAngle);

        public RenderStyles GetRenderStyle()
        {
            int key =   Instance.entity.GetInt(Key_RenderStyle, 0);
            if (key < 0 || key > (int)RenderStyles.UseMapTexture)
            {
                throw new FormatException($"{Instance.entity.ClassName} has bad RenderStyle {key}. expected 0, 1, or 2");
            }

            return (RenderStyles)key;
        }
    }
}
