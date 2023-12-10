using Pogo.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardPhysics.PhysicsTime;

public class RigorMortisAbility : PlayerAbility
{
    public Vector3 NewGravity;
    public float Drag;

    protected override void AppliedPhysicsUpdate()
    {
        // overcome gravity
        Owner.ApplyForce(Time.fixedDeltaTime * (NewGravity - Physics.gravity));
        ApplyDrag(Time.fixedDeltaTime);
    }

    protected override void AppliedRenderUpdate(RenderArgs arg0)
    {
    }

    protected override void OnApply()
    {
    }

    protected override void OnCleanse()
    {
    }

    private void ApplyDrag(float interval)
    {
        Owner.ApplyForce(Owner.Velocity.normalized * -1 * Drag * interval);
    }
}
