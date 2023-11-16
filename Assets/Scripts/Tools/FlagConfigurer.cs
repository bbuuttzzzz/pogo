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
        [HideInInspector]
        public float Radius = 2;
        [HideInInspector]
        public float FlagOffset = 0.5f;
        public Transform Flag1;
        public Transform Flag2;

        public Material FlagMaterial_Star;
        public Material FlagMaterial_NoStar;

        public ParticleLine[] ParticleLines;
        public BoxCollider Collider;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position - transform.right * Radius, transform.position + transform.right * Radius);
            Gizmos.DrawLine(transform.position - transform.right * Radius + transform.up * 0.05f, transform.position + transform.right * Radius+ transform.up * 0.05f);
        }
    }
}
