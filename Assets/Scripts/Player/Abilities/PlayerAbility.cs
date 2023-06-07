using System;
using UnityEngine;

namespace Pogo.Abilities
{
    public abstract class PlayerAbility : MonoBehaviour
    {
        protected PlayerController Owner;
        private bool isApplied;
        public bool IsApplied { get { return isApplied; } }
        public float DestroyDelay;

        public bool PreventDuplicates;

        public enum CleanseTypes
        {
            CleanseOnDeath,
            CleanseOnTouch
        }
        public CleanseTypes CleanseType;

        public void Apply(PlayerController target)
        {
            Owner = target;
            Owner.OnBeforeApplyAbility?.Invoke(this);
            Owner.OnTouch.AddListener(Target_OnTouch);
            Owner.OnSpawn.AddListener(Target_OnSpawn);
            Owner.OnBeforeApplyAbility.AddListener(Target_OnBeforeApplyAbility);
            isApplied = true;
            OnApply();
        }

        private void Target_OnBeforeApplyAbility(PlayerAbility arg0)
        {
            if (!PreventDuplicates) return;

            if (arg0.GetType() == this.GetType())
            {
                Cleanse();
            }
        }

        private void Target_OnSpawn()
        {
            Cleanse();
        }

        private void Target_OnTouch()
        {
            if (CleanseType == CleanseTypes.CleanseOnTouch)
            {
                Cleanse();
            }
        }

        public void Cleanse()
        {
            isApplied = false;
            OnCleanse();
            Owner.OnTouch.RemoveListener(Target_OnTouch);
            Owner.OnSpawn.RemoveListener(Target_OnSpawn);
            Owner.OnBeforeApplyAbility.RemoveListener(Target_OnBeforeApplyAbility);

            Owner = null;
            Destroy(gameObject, DestroyDelay);
        }

        protected virtual void Update()
        {
            if (IsApplied)
            {
                AppliedUpdate();
            }
        }

        protected abstract void OnApply();
        protected abstract void OnCleanse();
        protected abstract void AppliedUpdate();
    }
}
