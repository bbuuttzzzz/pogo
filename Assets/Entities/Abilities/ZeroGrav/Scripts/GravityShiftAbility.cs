using Players.Visuals;
using Pogo;
using Pogo.Abilities;
using UnityEngine;
using WizardEffects;
using WizardPhysics.PhysicsTime;

public class GravityShiftAbility : PlayerAbility
{
    public Vector3 NewGravity;
    public PlayerModelAttachment BackAttachment;
    public string EffectName;

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
        
        EffectData data = new EffectData();
        data.position = Owner.RenderPosition;
        WizardEffects.EffectManager.CreateEffect(EffectName, data);
    }

    protected override void OnCleanse()
    {
        Owner.AttachmentHandler.RemoveAttachment(BackAttachment);

        EffectData data = new EffectData();
        data.position = Owner.RenderPosition;
        WizardEffects.EffectManager.CreateEffect(EffectName, data);
    }
}
