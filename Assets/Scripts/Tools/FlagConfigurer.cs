using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardEffects;

namespace Pogo
{
    public class FlagConfigurer : MonoBehaviour
    {
        public float Radius = 2;
        public float FlagOffset = 0.5f;
        public Transform Flag1;
        public Transform Flag2;

        public ParticleLine[] ParticleLines;
        public BoxCollider Collider;

        private void OnValidate()
        {
            if (Flag1 != null) Flag1.localPosition = Vector3.left * (Radius - FlagOffset);
            if (Flag2 != null) Flag2.localPosition = Vector3.right * (Radius - FlagOffset);

            foreach(var particleLine in ParticleLines)
            {
                if (particleLine != null) particleLine.UpdateEffect();
            }
            if (Collider != null)
            {
                Vector3 size = Collider.size;
                size.x = Radius * 2;
                Collider.size = size;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position - transform.right * Radius, transform.position + transform.right * Radius);
            Gizmos.DrawLine(transform.position - transform.right * Radius + transform.up * 0.05f, transform.position + transform.right * Radius+ transform.up * 0.05f);
        }
    }
}
