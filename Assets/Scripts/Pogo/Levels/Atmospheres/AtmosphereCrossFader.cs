using Pogo.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Atmospheres
{
    public class AtmosphereCrossFader
    {
        private PogoLevelManager parent;
        public Atmosphere StartAtmosphere { get; private set; }
        public Atmosphere EndAtmosphere { get; private set; }
        public readonly ICrossFaderSettings Settings;
        public float CurrentProgress { get; private set; }

        public bool Finished => TaskProgress >= 1;
        public float TaskProgress { get; private set; }
        private Coroutine ActiveCoroutine;

        public AtmosphereCrossFader(
            PogoLevelManager parent,
            ICrossFaderSettings settings,
            Atmosphere startAtmosphere,
            Atmosphere endAtmosphere)
        {
            this.parent = parent;
            Settings = settings;
            StartAtmosphere = startAtmosphere;
            EndAtmosphere = endAtmosphere;
        }

        public void Reverse()
        {
            VerboseLog($"Reversing @ {CurrentProgress} -> {1 - Mathf.Clamp01(CurrentProgress)}");

            CurrentProgress = 1 - Mathf.Clamp01(CurrentProgress);

            (EndAtmosphere, StartAtmosphere) = (StartAtmosphere, EndAtmosphere);
            if (ActiveCoroutine == null)
            {
                BeginTransition();
            }
        }

        public void FinishNow()
        {
            if (ActiveCoroutine != null)
            {
                parent.StopCoroutine(ActiveCoroutine);

                // i think since this is not a unity object, it doesn't always set this null for us
                ActiveCoroutine = null;
            }
            StartAtmosphere.SetVolumeWeight(0);
            EndAtmosphere.FullyApply();
            RenderSettings.ambientLight = EndAtmosphere.AmbientLightColor;
            VerboseLog("Finish Now");
        }

        public void CleanUp()
        {
            if (StartAtmosphere != null)
            {
                StartAtmosphere.DisableAndDestroy();
            }
        }

        public void BeginTransition()
        {
            ActiveCoroutine = parent.StartCoroutine(TransitionAsync());
            VerboseLog("Begin Transition");
        }


        private IEnumerator TransitionAsync()
        {
            while (CurrentProgress < 1)
            {
                CurrentProgress += Time.unscaledDeltaTime / Settings.TransitionDuration;
                // find t on the transition curve, THEN orient the curve so it goes from start to end
                float t = Settings.TransitionCurve.Evaluate(CurrentProgress);
                StartAtmosphere.SetVolumeWeight(1 - t);
                EndAtmosphere.SetVolumeWeight(t);
                RenderSettings.ambientLight = Color.Lerp(StartAtmosphere.AmbientLightColor, EndAtmosphere.AmbientLightColor, t);
                RenderSettings.fogColor = Color.Lerp(StartAtmosphere.FogColor, EndAtmosphere.FogColor, t);
                RenderSettings.fogDensity = Mathf.Lerp(StartAtmosphere.FogDensity, EndAtmosphere.FogDensity, t);
                yield return null;
            }

            StartAtmosphere.SetVolumeWeight(0);
            EndAtmosphere.FullyApply();

            // i think since this is not a unity object, it doesn't set this null for us
            ActiveCoroutine = null;
            VerboseLog("End Transition");
        }

        private void VerboseLog(string text, bool includeHeader = true)
        {
            if (!Settings.VerboseLogging) return;

            if (includeHeader)
            {
                Debug.Log($"{this}: {text}");
            }
            else
            {
                Debug.Log(text);
            }
        }

        public override string ToString()
        {

            string name1 = StartAtmosphere != null ? StartAtmosphere.name : "NULL";
            string name2 = EndAtmosphere != null ? EndAtmosphere.name : "NULL";
            return $"Crossfader {name1} -> {name2}";
        }

        public interface ICrossFaderSettings
        {
            public AnimationCurve TransitionCurve { get; }
            public float TransitionDuration { get; }
            public bool VerboseLogging { get; }
        }
    }
}
