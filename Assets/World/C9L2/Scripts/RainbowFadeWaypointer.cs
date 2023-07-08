using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils;

[RequireComponent(typeof(Renderer))]
public class RainbowFadeWaypointer : Waypointer<float>
{
    private Renderer Target;
    private int FadeParameterId;

    public override void Awake()
    {
        base.Awake();
        Target = GetComponent<Renderer>();
        FadeParameterId = Shader.PropertyToID("_Intensity");
    }

    private float _value;

    public float Value
    {
        get => _value; set
        {
            _value = value;
            Target.materials[0].SetFloat(FadeParameterId, value);

        }
    }

    protected override float GetCurrentValue() => Value;

    protected override void InterpolateAndApply(float startValue, float endValue, float i)
    {
        Value = Mathf.Lerp(startValue, endValue, i);
    }
}
