using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Func_Train : WrappedEntityInstance
    {
        private const string Key_TrackStart = "trackstart";
        private const string Key_Count = "carcount";
        private const string Key_Surface = "surface";

        public Func_Train(BSPLoader.EntityInstance instance, IBSPLoaderContext context) : base("func_train", instance, context)
        {
        }

        public BSPLoader.EntityInstance GetTrackStart() => GetSingleTarget(Key_TrackStart);
        public int GetCarCount() => Instance.entity.GetInt(Key_Count);
        public string GetSurface() => Instance.entity[Key_Surface];
    }
}
