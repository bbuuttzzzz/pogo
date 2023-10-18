using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlideParticleSystemController : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem Target;

    private float EmissionRate;
    private PlayerController Parent;

    public void Awake()
    {
        UpdateSettingsFromTarget();
    }

    [ContextMenu("Update Settings from Target")]
    private void UpdateSettingsFromTarget()
    {
        EmissionRate = Target.emission.rateOverTime.constant;
    }

    float emissionBudget = 0;

    public void SetParent(PlayerController parent)
    {
        Parent = parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Parent == null)
        {
            return;
        }

        emissionBudget += Time.deltaTime * EmissionRate;

        while (emissionBudget > 0)
        {
            Debug.Log("Emit!");
            emissionBudget--;
            EmitParticles(1);
        }
    }

    private void EmitParticles(int count)
    {
        ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
        Target.Emit(p, count);
    }
}
