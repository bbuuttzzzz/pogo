using Pogo;
using System;
using UnityEngine;
using WizardPhysics.PhysicsTime;

namespace WizardUtils
{
    public abstract class PhysicsTimeWaypointer<T> : MonoBehaviour
    {
        public T[] Waypoints;
        private float CurrentPhysicsTime;

        /// <summary>
        /// What waypoint to start at. -1 is to stay in its original position
        /// </summary>
        public int InitialWaypointIndex = -1;

        [Tooltip("how long the transition will take in seconds")]
        public float TransitionDuration = 10;

        T initialWaypointValue;
        T lastWaypointValue;
        T nextWaypointValue;

        T LastPhysicsValue;
        T CurrentPhysicsValue;

        private enum States
        {
            Moving,
            PhysicsArrived,
            RenderArrived
        };
        private States state = States.Moving;

        private float changeStartPhysicsTime;

        public bool UseCustomCurve;
        public AnimationCurve CustomCurve;

        public virtual void Awake()
        {
            initialWaypointValue = GetCurrentPhysicsValue();
            SnapToWaypoint(InitialWaypointIndex);
        }

        public virtual void Start()
        {
            PogoGameManager.PogoInstance.TimeManager.OnPhysicsUpdate.AddListener(OnPhysicsUpdate);
            PogoGameManager.PogoInstance.TimeManager.OnRenderUpdate.AddListener(OnRenderUpdate);
        }

        public virtual void OnDestroy()
        {
            PogoGameManager.PogoInstance.TimeManager.OnPhysicsUpdate.RemoveListener(OnPhysicsUpdate);
            PogoGameManager.PogoInstance.TimeManager.OnRenderUpdate.RemoveListener(OnRenderUpdate);
        }

        private void OnRenderUpdate(RenderArgs arg0)
        {
            if (state != States.RenderArrived)
            {
                InterpolateAndApplyRender(LastPhysicsValue, CurrentPhysicsValue, arg0.FrameInterpolator);
            }
        }

        private void OnPhysicsUpdate()
        {
            CurrentPhysicsTime += Time.fixedDeltaTime;
            if (state == States.Moving)
            {
                float i = ConvertParametric((CurrentPhysicsTime - changeStartPhysicsTime) / TransitionDuration);
                InterpolateAndApplyPhysics(lastWaypointValue, nextWaypointValue, i);
                if (CurrentPhysicsTime > changeStartPhysicsTime + TransitionDuration)
                {
                    state = States.PhysicsArrived;
                }

                LastPhysicsValue = CurrentPhysicsValue;
                CurrentPhysicsValue = GetCurrentPhysicsValue();
            }
            else if (state == States.PhysicsArrived)
            {
                LastPhysicsValue = CurrentPhysicsValue;

                InterpolateAndApplyRender(CurrentPhysicsValue, CurrentPhysicsValue, 1);
                state = States.RenderArrived;
            }
        }


        private float ConvertParametric(float rawParametric)
        {
            if (UseCustomCurve)
            {
                return CustomCurve.Evaluate(rawParametric);
            }
            else
            {
                return rawParametric;
            }
        }

        /// <summary>
        /// Get the value we're currently at
        /// </summary>
        /// <returns></returns>
        protected abstract T GetCurrentPhysicsValue();

        /// <summary>
        /// Move the target to the value Interpolated between <paramref name="startValue"/> and <paramref name="endValue"/> by <paramref name="i"/>
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="i">a value from 0-1. if 0, returns </param>
        protected abstract void InterpolateAndApplyPhysics(T startValue, T endValue, float i);
        protected abstract void InterpolateAndApplyRender(T startValue, T endValue, float i);


        public void GoToWaypoint(int waypointIndex)
        {
            lastWaypointValue = GetCurrentPhysicsValue();
            nextWaypointValue = GetWaypoint(waypointIndex);
            changeStartPhysicsTime = CurrentPhysicsTime;
            state = States.Moving;
        }

        private T GetWaypoint(int waypointIndex)
        {
            if (waypointIndex < 0 || waypointIndex >= Waypoints.Length) return initialWaypointValue;
            return Waypoints[waypointIndex];
        }

        public void SnapToWaypoint(int waypointIndex)
        {
            GoToWaypoint(waypointIndex);
            FinishNow();
        }

        public void FinishNow()
        {
            InterpolateAndApplyPhysics(lastWaypointValue, nextWaypointValue, 1);
            CurrentPhysicsValue = GetCurrentPhysicsValue();
            LastPhysicsValue = CurrentPhysicsValue;
            InterpolateAndApplyRender(CurrentPhysicsValue, CurrentPhysicsValue, 1);
            state = States.RenderArrived;
        }
    }
}
