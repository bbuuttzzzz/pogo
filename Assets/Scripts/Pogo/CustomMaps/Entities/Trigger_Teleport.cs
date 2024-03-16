using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    internal class Trigger_Teleport : WrappedCreatedEntity
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

        public Trigger_Teleport(BSPLoader.EntityCreatedCallbackData data) : base("trigger_teleport", data)
        {
        }

        public bool GetPreservePhysics() => Data.Instance.entity.GetInt(Key_PreservePhysics) > 0;
        public float GetPreservePhysicsAngle() => Data.Instance.entity.GetInt(Key_PreservePhysicsAngle);

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
