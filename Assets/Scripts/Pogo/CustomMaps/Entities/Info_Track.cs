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

        public string GetNextTrackName() => Instance.entity[Key_Next];
        public BSPLoader.EntityInstance? GetNextTrackOrDefault()
        {
            if (string.IsNullOrEmpty(GetNextTrackName())) return null;

            return GetSingleTarget(Key_Next);
        }

        public StopData GetStopData()
        {
            StopData stopData = new()
            {
                StopTime = Instance.entity.GetInt(Key_StopTime) / 1000,
                TravelTime = Instance.entity.GetInt(Key_TravelTime) / 1000,
                EasingType = GetEasing(),
                Position = Instance.gameObject.transform.position
            };

            return stopData;
        }

        public EasingTypes GetEasing()
        {
            int key = Instance.entity.GetInt(Key_Easing, 0);
            if (key < 0 || key > (int)EasingTypes.EaseInAndOut)
            {
                throw new FormatException($"{Instance.entity.ClassName} has bad Easing {key}. expected 0-3");
            }

            return (EasingTypes)key;
        }
    }
}
