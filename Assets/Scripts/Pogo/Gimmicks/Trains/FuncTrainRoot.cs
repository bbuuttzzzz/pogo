using Pogo.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Trains
{
    public class FuncTrainRoot : MonoBehaviour
    {
        public GameObject TrainCarPrefab;
        private List<StopData> Stops;
        private TrainTrack Track;

        private void Awake()
        {
            Stops = new List<StopData>();
        }

        public void AddStop(StopData stop) => Stops.Add(stop);

        public void FinishTrack(int carsCount, SurfaceConfig surface)
        {
            Track = new TrainTrack(name, Stops.ToArray());
            CreateTrainCars(carsCount, surface);
            CleanupRootVisuals();
        }

        private void CreateTrainCars(int count, SurfaceConfig surface)
        {
            MeshRenderer localRenderer = GetComponent<MeshRenderer>();

            for (int n = 0; n < count; n++)
            {
                var carObject = Instantiate(TrainCarPrefab, transform);
                var car = carObject.GetComponent<TrainCar>();
                car.Track = Track;
                car.Offset(n / (float)count);
                car.CopyVisuals(localRenderer, surface);
            }
        }

        private void CleanupRootVisuals()
        {
            Destroy(GetComponent<Renderer>());
            Destroy(GetComponent<MeshCollider>());
        }
    }
}
        