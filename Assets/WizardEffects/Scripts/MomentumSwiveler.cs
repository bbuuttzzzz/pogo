using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardEffects
{
    /// <summary>
    /// Takes all translation applied to this gameobject, and applies it with a delay & other parameters to <paramref name="target"/>
    /// </summary>
    public class MomentumSwiveler : MonoBehaviour
    {
        public Transform target;

        [Tooltip("clamp accumulated motion to this radius")]
        public float MaxTrackedRadius = 1;

        [Tooltip("the target is moved around as if the accumulated movement was shrinked to a sphere of this radius")]
        public float MaxShownRadius = 0.5f;

        [Tooltip("Speed at which the swiveler returns to its original position")]
        public float DecayRate = 10;

        Vector3 accumulation;

        private void Update()
        {
            accumulateMotion();
            applyAccumulaion();
        }

        private void applyAccumulaion()
        {
            target.transform.position = transform.position + accumulation * (MaxShownRadius / MaxTrackedRadius);
        }

        Vector3 lastPosition;
        private void accumulateMotion()
        {
            // add motion from this frame to accumulation
            accumulation -= target.transform.position - lastPosition;
            lastPosition = target.transform.position;

            // apply decay
            float magnitude = accumulation.magnitude;
            float loseMagnitude = DecayRate * Time.deltaTime;

            if (magnitude <= loseMagnitude)
            {
                accumulation = Vector3.zero;
            }
            else
            {
                accumulation *= (magnitude - loseMagnitude) / magnitude;
            }


            // then clamp
            float squareMagnitude = accumulation.sqrMagnitude;
            if (squareMagnitude > MaxTrackedRadius * MaxTrackedRadius)
            {
                accumulation *= MaxTrackedRadius / Mathf.Sqrt(squareMagnitude);
            }
        }

        public void Disjoint()
        {
            lastPosition = target.transform.position;
            accumulation = Vector3.zero;
            applyAccumulaion();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, MaxTrackedRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, MaxShownRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.transform.position, 0.02f);
        }
    }
}
