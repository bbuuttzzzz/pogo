using Players.Visuals;
using System;
using UnityEngine;
using WizardPhysics.PhysicsTime;
using WizardUtils;

namespace Pogo.Abilities
{
    public class GlideAbility : PlayerAbility
    {
        public float LiftScale = 1;
        public AnimationCurve LiftCurve;
        public float DragScale = 1;
        public AnimationCurve DragCurve;
        public Transform WingsTransform;
        public float RudderScale = 0.25f;

        public PlayerModelAttachment BackAttachment;

        protected override void AppliedPhysicsUpdate()
        {
            ApplyLift(Time.fixedDeltaTime);
            ApplyDragAndRudder(Time.fixedDeltaTime);
            WingsTransform.rotation = Owner.DesiredModelRotation;
        }

        protected override void AppliedRenderUpdate(RenderArgs arg0)
        {
        }

        private void ApplyLift(float interval)
        {
            float evaluatedLift = LiftCurve.Evaluate(Owner.AngleOfAttack) * LiftScale;
            float squareVelocityAlongForward = Square(Vector3.Dot(Owner.ModelForward, Owner.Velocity));

            Owner.ApplyForce(Owner.ModelUp * evaluatedLift * squareVelocityAlongForward * interval);
        }

        private void ApplyDragAndRudder(float interval)
        {
            float evaluatedDrag = DragCurve.Evaluate(Owner.AngleOfAttack) * DragScale;
            float squareSpeed = Owner.Velocity.sqrMagnitude;

            Owner.ApplyForce(Owner.Velocity.normalized * -1 * evaluatedDrag * squareSpeed * interval);
            Owner.ApplyForce(Owner.ModelForward * evaluatedDrag * squareSpeed * interval * RudderScale);
        }

        protected override void OnApply()
        {
            Owner.AttachmentHandler.AddAttachment(BackAttachment);
        }

        protected override void OnCleanse()
        {
            transform.parent = null;
            Owner.AttachmentHandler.RemoveAttachment(BackAttachment);
        }

        private float Square(float value) => value * value;
    }
}
