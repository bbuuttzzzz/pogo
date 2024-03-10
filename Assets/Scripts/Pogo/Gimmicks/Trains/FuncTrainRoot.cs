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

        public void FinishTrack(int carsCount)
        {
            Track = new TrainTrack(name, Stops.ToArray());
            CreateTrainCars(carsCount);
            CleanupRootVisuals();
        }

        private void CreateTrainCars(int count)
        {
            MeshRenderer localRenderer = GetComponent<MeshRenderer>();

            for (int n = 0; n < count; n++)
            {
                var carObject = Instantiate(TrainCarPrefab, transform);
                var car = carObject.GetComponent<TrainCar>();
                car.Offset(n / count);
                car.CopyVisuals(localRenderer);
                car.Track = Track;
            }
        }

        private void CleanupRootVisuals()
        {
            Destroy(GetComponent<Renderer>());
            Destroy(GetComponent<MeshCollider>());
        }
    }
}
        