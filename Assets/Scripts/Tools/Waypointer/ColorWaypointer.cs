using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardTools
{
    public class ColorWaypointer : Waypointer<Color>
    {
        public Renderer renderer;

        protected override Color GetCurrentValue()
        {
            return renderer.material.color;
        }

        protected override void InterpolateAndApply(Color startValue, Color endValue, float i)
        {
            Color newColor = Color.Lerp(startValue, endValue, i);
            renderer.material.color = newColor;
        }
    }
}

