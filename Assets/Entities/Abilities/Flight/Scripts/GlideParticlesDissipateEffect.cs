using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GlideParticlesDissipateEffect : MonoBehaviour
{
    new ParticleSystem particleSystem;

    public void Activate()
    {
        particleSystem = GetComponent<ParticleSystem>();
        InterpolateOverTime(2,(t) =>
        {
            var emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = 1 - t;
            var main = particleSystem.main;
            main.startSpeed = t;
            var noise = particleSystem.noise;
            noise.strengthMultiplier = 1 + t * 4;
        });

    }

    
    public void InterpolateOverTime(float duration, Action<float> action)
    {
        StartCoroutine(InterpolateActionOverTime(action, duration));
    }

    private IEnumerator InterpolateActionOverTime(Action<float> action, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            action.Invoke((Time.time - startTime) / duration);
            yield return null;
        }

        action.Invoke(1);
    }
}
