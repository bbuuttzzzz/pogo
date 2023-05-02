using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShaderFloatUpdater : MonoBehaviour
{
    new Renderer renderer;
    private int ParameterId;
    public string ParameterName;

    private void Awake()
    {
        ParameterId = Shader.PropertyToID(ParameterName);
        renderer = GetComponent<Renderer>();
        cachedValue = CurrentValue;
    }

    public float CurrentValue;
    float cachedValue;

    private void LateUpdate()
    {
        if (CurrentValue == cachedValue) return;

        cachedValue = CurrentValue;
        renderer.material.SetFloat(ParameterId, CurrentValue);
    }
}
