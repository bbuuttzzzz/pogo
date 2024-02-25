using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardEffects;

namespace Pogo.Gimmicks
{
    public class ColliderSplashEffect : MonoBehaviour
    {
        public EffectTemplate Effect;

        public void Trigger(Collider other)
        {
            if (!TryFindOriginFrom(other, out FindOriginResult result))
            {
                result.origin = GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center);
                result.normal = (other.bounds.center - result.origin).normalized;
            }

            WizardEffects.EffectManager.CreateEffect(Effect.name, new EffectData()
            {
                position = result.origin,
                normal = result.normal,
            });
        }

        private bool TryFindOriginFrom(Collider other, out FindOriginResult result)
        {
            Collider myCollider = GetComponent<Collider>();

            if (Physics.ComputePenetration(
                other, other.transform.position, other.transform.rotation,
                myCollider, myCollider.transform.position, myCollider.transform.rotation,
                out Vector3 direction,
                out float distance))
            {
                result = new FindOriginResult
                {
                    normal = direction,
                    origin = other.ClosestPointOnBounds(other.bounds.center - 100 * distance * direction)
                };
                return true;
            }
            result = default;
            return false;
        }

        private struct FindOriginResult
        {
            public Vector3 origin;
            public Vector3 normal;
        }
    }
}
