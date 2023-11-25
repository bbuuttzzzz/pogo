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
            CurrentProgress = 1 - Mathf.Clamp01(CurrentProgress);

            Atmosphere tempAtmosphere = StartAtmosphere;
            StartAtmosphere = EndAtmosphere;
            EndAtmosphere = tempAtmosphere;

            if (ActiveCoroutine == null)
            {
                BeginTransition();
            }
        }

        public void FinishNow()
        {
            parent.StopCoroutine(ActiveCoroutine);
            StartAtmosphere.SetWeight(0);
            EndAtmosphere.SetWeight(1);
            RenderSettings.ambientLight = EndAtmosphere.AmbientLightColor;
        }

        public void CleanUp()
        {
            StartAtmosphere.DisableAndDestroy();
        }

        public void BeginTransition()
        {
            ActiveCoroutine = parent.StartCoroutine(TransitionAsync());
        }


        private IEnumerator TransitionAsync()
        {
            while (CurrentProgress < 1)
            {
                CurrentProgress += Time.unscaledDeltaTime / Settings.TransitionDuration;
                // find t on the transition curve, THEN orient the curve so it goes from start to end
                float t = Settings.TransitionCurve.Evaluate(CurrentProgress);
                StartAtmosphere.SetWeight(1 - t);
                EndAtmosphere.SetWeight(t);
                RenderSettings.ambientLight = Color.Lerp(StartAtmosphere.AmbientLightColor, EndAtmosphere.AmbientLightColor, t);
                yield return null;
            }

            StartAtmosphere.SetWeight(0);
            EndAtmosphere.SetWeight(1);
            RenderSettings.ambientLight = EndAtmosphere.AmbientLightColor;
        }

        public interface ICrossFaderSettings
        {
            public AnimationCurve TransitionCurve { get; }
            public float TransitionDuration { get; }
        }
    }
}
