using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class Atmosphere : MonoBehaviour
{
    Volume volume;

    public float FadeTime = 0.2f;
    public AnimationCurve TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [ColorUsage(false, true)]
    public Color AmbientLightColor = Color.white;

    public Light TargetLight;
    private float DefaultLightIntensity;

    public bool EnableVerboseLogging;

    private void Awake()
    {
        if (TargetLight != null)
        {
            DefaultLightIntensity = TargetLight.intensity;
            TargetLight.intensity = 0;
        }
        volume = GetComponent<Volume>();
        volume.weight = 0;
        currentWeight = 0;
    }

    float currentWeight;

    public void SetWeight(float weight, bool instant)
    {
        if (EnableVerboseLogging)
        {
            string end = instant ? " instantly" : "";
            Debug.Log($"{name} Weight set to {weight}{end}, this");
        }

        if (instant)
        {
            setWeight(1);
            RenderSettings.ambientLight = AmbientLightColor;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateWeight(currentWeight, weight, FadeTime));
        }
    }

    public void SetWeightFromEditor(float weight)
    {
        volume = GetComponent<Volume>();
        SetWeight(weight, true);
        RenderSettings.ambientLight = AmbientLightColor;
    }

    public void DisableAndDestroy(bool instant)
    {
        if (instant)
        {
            setWeight(0);
            Destroy(gameObject);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(InterpolateWeight(currentWeight, 0f, FadeTime, false));
            Destroy(gameObject, FadeTime + 1);
        }
    }

    IEnumerator InterpolateWeight(float start, float end, float duration, bool updateLighting = true)
    {
        float startTime = Time.time;
        Color startColor = RenderSettings.ambientLight;

        while(Time.time < startTime + duration)
        {
            // find t on the transition curve, THEN orient the curve so it goes from start to end
            float t = TransitionCurve.Evaluate((Time.time - startTime) / duration);
            t = Mathf.Lerp(start, end, t);
            // doing it in the opposite order would read TransitionCurve backwards when going from 1 to 0

            if (updateLighting)
            {
                var color = Color.Lerp(startColor, AmbientLightColor, t);
                RenderSettings.ambientLight = color;
            }
            setWeight(t);
            yield return null;
        }

        if (updateLighting) RenderSettings.ambientLight = AmbientLightColor;
        setWeight(end);
    }

    void setWeight(float t)
    {
        currentWeight = t;
        volume.weight = t;
        SetLightWeight(t);
    }

    private void SetLightWeight(float t)
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying) return;
#endif


        if (TargetLight != null)
        {
            TargetLight.intensity = DefaultLightIntensity * t;
        }
    }
}
