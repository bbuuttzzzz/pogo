using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pogo.Atmospheres
{
    [RequireComponent(typeof(Volume))]
    public class Atmosphere : MonoBehaviour
    {
        Volume volume;

        [ColorUsage(false, true)]
        public Color AmbientLightColor = Color.white;

        public Light TargetLight;
        private float DefaultLightIntensity;

        public Color FogColor = Color.white;
        public float FogDensity = 0f;

        [NonSerialized]
        public GameObject SelfPrefab;

        private void Awake()
        {
            if (TargetLight != null)
            {
                DefaultLightIntensity = TargetLight.intensity;
                TargetLight.intensity = 0;
            }
            volume = GetComponent<Volume>();
            volume.weight = 0;
        }

        public void SetWeight(float t)
        {
            volume.weight = t;
            SetLightWeight(t);
        }

        public void SetMaxWeightFromEditor()
        {
            volume = GetComponent<Volume>();
            SetWeight(1);
            RenderSettings.ambientLight = AmbientLightColor;
            RenderSettings.fog = true;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogDensity = FogDensity;
        }

        public void DisableAndDestroy()
        {
            SetWeight(0);
            Destroy(gameObject);
        }

        private void SetLightWeight(float t)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
#endif

            if (TargetLight != null)
            {
                TargetLight.intensity = DefaultLightIntensity * t;
            }
        }
    }
}