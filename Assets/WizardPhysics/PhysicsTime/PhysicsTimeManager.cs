#pragma warning disable UNT0004 // Time.fixedDeltaTime used with Update
using UnityEngine;
using UnityEngine.Events;

namespace WizardPhysics.PhysicsTime
{
    public class PhysicsTimeManager : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnPhysicsUpdate;
        [HideInInspector]
        public UnityEvent<RenderArgs> OnRenderUpdate;
        const float MaximumAllowedFrameTime = 0.25f;

        private float Accumulator;
        void Update()
        {
            if (Time.deltaTime == 0) return;

            Accumulator += Mathf.Min(Time.deltaTime, MaximumAllowedFrameTime);
            while (Accumulator > Time.fixedDeltaTime)
            {
                Accumulator -= Time.fixedDeltaTime;
                OnPhysicsUpdate.Invoke();
            }

            float t = Accumulator / Time.fixedDeltaTime;

            OnRenderUpdate.Invoke(new RenderArgs(t));
        }
    }
}
#pragma warning restore UNT0004 // Time.fixedDeltaTime used with Update