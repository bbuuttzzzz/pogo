using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public class Func_Train : WrappedCreatedEntity
    {
        private const string Key_TrackStart = "trackstart";
        private const string Key_Count = "carcount";
        private const string Key_Surface = "surface";

        public Func_Train(BSPLoader.EntityCreatedCallbackData data) : base("func_train", data)
        {
        }

        public BSPLoader.EntityInstance GetTrackStart() => GetSingleTarget(Key_TrackStart);
        public int GetCarCount() => Data.Instance.entity.GetInt(Key_Count);
        public string GetSurface() => Data.Instance.entity[Key_Surface];
    }
}
