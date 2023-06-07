using System;
using UnityEngine;
using WizardUtils.Extensions;

namespace Pogo
{
    public class CapsuleBlowerConfigurer : MonoBehaviour
    {
        public float Height;
        public float BottomOffset;
        public float Radius;
        public CapsuleCollider Capsule;
        public ParticleSystem WindParticles;

        private void OnValidate()
        {
            if (Capsule != null) ConfigureCapsule();
            if (WindParticles != null) ConfigureParticles();
        }

        const float TimePerMeter = 0.1f;
        private void ConfigureParticles()
        {
            var shape = WindParticles.shape;
            shape.radius = Radius;

            var main = WindParticles.main;
            main.startLifetime = TimePerMeter * Height;
        }

        private void ConfigureCapsule()
        {
            Capsule.height = Height + BottomOffset;
            Capsule.radius = Radius;
            Capsule.center = new Vector3(0, 0, (Height - BottomOffset) / 2);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * Height);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * BottomOffset);
            Gizmos.color = Color.white;
            GizmosHelper.DrawCircle(transform.position, transform.rotation * Quaternion.Euler(90, 0, 0), Radius);
        }
    }
}
