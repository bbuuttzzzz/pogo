using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlideParticleSystemController : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem Target;

    private Transform lastOrientationTransform;
    private Vector3 lastOrientationPosition;
    private Quaternion lastOrientationRotation;
    public AnimationCurve SpaceChangeCurve;

    ParticleSystem.Particle[] particles;
    int currentParticleCount = 0;

    public void Awake()
    {
        lastOrientationTransform = new GameObject("lastOrientation").transform;
        lastOrientationTransform.parent = transform;

        particles = new ParticleSystem.Particle[Target.main.maxParticles];
    }


    // Update is called once per frame
    void Update()
    {
        currentParticleCount = Target.GetParticles(particles);
        lastOrientationTransform.position = lastOrientationPosition;
        lastOrientationTransform.rotation = lastOrientationRotation;

        for(int n = 0; n < currentParticleCount; n++)
        {
            Vector3 previousLocalPosition = lastOrientationTransform.InverseTransformPoint(particles[n].position);
            Vector3 nextRelativeWorldPosition = transform.TransformPoint(previousLocalPosition);
            Vector3 deltaWorldPosition = nextRelativeWorldPosition - particles[n].position;

            float tRaw = 1 - particles[n].remainingLifetime / particles[n].startLifetime;

            Vector3 finalPosition = Vector3.Lerp(particles[n].position, particles[n].position + deltaWorldPosition, SpaceChangeCurve.Evaluate(tRaw));
            particles[n].position = finalPosition;

        }

        Target.SetParticles(particles);
        lastOrientationPosition = transform.position;
        lastOrientationRotation = transform.rotation;
    }
}
