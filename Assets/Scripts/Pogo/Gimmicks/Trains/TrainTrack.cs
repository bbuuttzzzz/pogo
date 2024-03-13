using UnityEngine;
using WizardUtils.Extensions;

namespace Pogo.Trains
{
    public class TrainTrack
    {
        public string Name { get; private set; }
        public float TotalDuration { get; private set; }
        private StopData[] Stops;

        public TrainTrack(string name, StopData[] stops)
        {
            Name = name;
            Stops = stops;
            foreach(var stop in Stops)
            {
                TotalDuration += stop.TravelTime + stop.StopTime;
            }
        }

        public Vector3 Origin => Stops[0].Position;

        public Vector3 Sample(float time)
        {
            if (TotalDuration == 0)
            {
                Debug.LogWarning($"TrainTrack {Name} has total duration of zero milliseconds!");
                return Stops[0].Position;
            }
            float _time = time.PositiveModulo(TotalDuration);

            for (int i = 0; i < Stops.Length; i++)
            {
                StopData stop = Stops[i];
                if (_time < stop.StopTime)
                {
                    return stop.Position;
                }
                _time -= stop.StopTime;

                if (_time < stop.TravelTime)
                {
                    StopData nextStop = Stops[(i+1).PositiveModulo(Stops.Length)];
                    float t = _time / stop.TravelTime;
                    return Interpolate(stop.Position, nextStop.Position, stop.EasingType, t);
                }
                _time -= stop.TravelTime;
            }

            Debug.LogWarning($"TrainTrack {Name}'s time argument somehow out of range? ({time}) returning default");
            return Stops[0].Position;
        }

        private static Vector3 Interpolate(Vector3 start, Vector3 end, EasingTypes easing, float t)
        {
            switch(easing)
            {
                // linear is a NOOP
                case EasingTypes.EaseIn:
                    t = t * t;
                    break;
                case EasingTypes.EaseOut:
                    t = 1 - (1 - t) * (1 - t);
                    break;
                case EasingTypes.EaseInAndOut:
                    if (t < 0.5)
                    {
                        t = 2 * t * t;
                    }
                    else
                    {
                        float pow = (-2 * t + 2);
                        t = 1 - pow * pow / 2;
                    }
                    break;
            }

            return Vector3.Lerp(start, end, t);
        }
    }
}
