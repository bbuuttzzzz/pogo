using BSPImporter;
using Pogo.Trains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Info_Track : WrappedEntityInstance
    {
        private const string Key_Next = "target";
        private const string Key_StopTime = "stoptime";
        private const string Key_TravelTime = "traveltime";
        private const string Key_Easing = "easing";

        protected Info_Track(string className, BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base (className, instance, context)
        {
        
        }

        public Info_Track(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("info_track", instance, context)
        {
        }

        public string GetNextTrackName() => GetValueOrDefault(Key_Next, "");
        public BSPLoader.EntityInstance? GetNextTrackOrDefault()
        {
            if (string.IsNullOrEmpty(GetNextTrackName())) return null;

            return GetSingleTarget(Key_Next);
        }

        public StopData GetStopData()
        {
            StopData stopData = new()
            {
                StopTime = GetIntOrDefault(Key_StopTime, 1000, minValue: 0) / 1000,
                TravelTime = GetIntOrDefault(Key_TravelTime, 1000, minValue: 0) / 1000,
                EasingType = GetEasing(),
                Position = InstanceGameObject.transform.position
            };

            return stopData;
        }

        public EasingTypes GetEasing()
        {
            int key = GetIntOrDefault(Key_Easing, 0, 0, (int)EasingTypes.EaseInAndOut);
            return (EasingTypes)key;
        }
    }
}
