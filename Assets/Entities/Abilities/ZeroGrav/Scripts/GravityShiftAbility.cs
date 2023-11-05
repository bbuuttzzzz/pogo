using Players.Visuals;
using Pogo.Abilities;
using UnityEngine;
using WizardPhysics.PhysicsTime;

public class GravityShiftAbility : PlayerAbility
{
    public Vector3 NewGravity;
    public PlayerModelAttachment BackAttachment;

    protected override void AppliedPhysicsUpdate()
    {
        Owner.ApplyForce(Time.fixedDeltaTime * (NewGravity - Physics.gravity));
    }

    protected override void AppliedRenderUpdate(RenderArgs arg0)
    {
    }

    protected override void OnApply()
    {
        Owner.AttachmentHandler.AddAttachment(BackAttachment);
    }

    protected override void OnCleanse()
    {
        Owner.AttachmentHandler.RemoveAttachment(BackAttachment);
    }
}
