using System;
using UnityEngine;

namespace WizardUtils
{
    public abstract class Waypointer<T> : MonoBehaviour
    {

        public T[] Waypoints;

        /// <summary>
        /// What waypoint to start at. -1 is to stay in its original position
        /// </summary>
        public int InitialWaypointIndex = -1;

        [Tooltip("how long the transition will take in seconds")]
        public float TransitionDuration = 10;

        T initialValue;
        T lastValue;
        T nextValue;

        bool arrived;
        private float changeStartTime;

        public virtual void Awake()
        {
            initialValue = GetCurrentValue();
            SnapToWaypoint(InitialWaypointIndex);
        }

        public virtual void Update()
        {
            if (!arrived)
            {
                InterpolateAndApply(lastValue, nextValue, (Time.time - changeStartTime) / TransitionDuration);
                if (Time.time > changeStartTime + TransitionDuration)
                {
                    arrived = true;
                }
            }
        }

        /// <summary>
        /// Get the value we're currently at
        /// </summary>
        /// <returns></returns>
        protected abstract T GetCurrentValue();

        /// <summary>
        /// Move the target to the value Interpolated between <paramref name="startValue"/> and <paramref name="endValue"/> by <paramref name="i"/>
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="i">a value from 0-1. if 0, returns </param>
        protected abstract void InterpolateAndApply(T startValue, T endValue, float i);

        public void GoToWaypoint(int waypointIndex)
        {
            lastValue = GetCurrentValue();
            nextValue = GetWaypoint(waypointIndex);
            changeStartTime = Time.time;
            arrived = false;
        }

        private T GetWaypoint(int waypointIndex)
        {
            if (waypointIndex < 0 || waypointIndex >= Waypoints.Length) return initialValue;
            return Waypoints[waypointIndex];
        }

        public void SnapToWaypoint(int waypointIndex)
        {
            GoToWaypoint(waypointIndex);
            FinishNow();
        }

        public void FinishNow()
        {
            InterpolateAndApply(lastValue, nextValue, 1);
            arrived = true;
        }
    }
}
