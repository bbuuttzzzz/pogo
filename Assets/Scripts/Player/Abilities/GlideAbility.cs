using System;
using UnityEngine;

namespace Pogo.Abilities
{
    public class GlideAbility : PlayerAbility
    {
        public float LiftScale = 1;
        public AnimationCurve LiftCurve;
        public float DragScale = 1;
        public AnimationCurve DragCurve;
        public Transform WingsTransform;

        protected override void AppliedUpdate()
        {
            ApplyLift();
            ApplyDrag();
            WingsTransform.rotation = Owner.DesiredModelRotation;
        }

        private void ApplyLift()
        {
            float evaluatedLift = LiftCurve.Evaluate(Owner.AngleOfAttack) * LiftScale;
            float squareVelocityAlongForward = Square(Vector3.Dot(Owner.ModelForward, Owner.Velocity));

            Owner.ApplyForce(Owner.ModelUp * evaluatedLift * squareVelocityAlongForward * Time.deltaTime);
        }

        private void ApplyDrag()
        {
            float evaluatedDrag = DragCurve.Evaluate(Owner.AngleOfAttack) * DragScale;
            float squareSpeed = Owner.Velocity.sqrMagnitude;

            Owner.ApplyForce(Owner.Velocity.normalized * -1 * evaluatedDrag * squareSpeed * Time.deltaTime);
        }

        protected override void OnApply()
        {
            Debug.Log("Applied");
        }

        protected override void OnCleanse()
        {
            Debug.Log("Cleansed");
        }

        private float Square(float value) => value * value;
    }
}
