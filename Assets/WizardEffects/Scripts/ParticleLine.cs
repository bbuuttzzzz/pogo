using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardEffects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleLine : MonoBehaviour
    {
        public Transform start;
        public Transform end;

        public float EmissionRatePerMeter;

        public void UpdateEffect()
        {
            ParticleSystem system = GetComponent<ParticleSystem>();

            float distance = (start.transform.position - end.transform.position).magnitude;
            transform.position = start.transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(end.transform.position - start.transform.position, Vector3.up));

            var shapeModule = system.shape;
            shapeModule.radius = distance / 2;
            shapeModule.position = Vector3.right * distance/2;

            var emissionModule = system.emission;
            emissionModule.rateOverTime = EmissionRatePerMeter * distance;
        }
    }

}