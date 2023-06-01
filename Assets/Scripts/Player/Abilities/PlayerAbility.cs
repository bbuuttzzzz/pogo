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

        public enum CleanseTypes
        {
            CleanseOnDeath,
            CleanseOnTouch
        }
        public CleanseTypes CleanseType;

        public void Apply(PlayerController target)
        {
            Owner = target;
            Owner.OnTouch.AddListener(Target_OnTouch);
            Owner.OnSpawn.AddListener(Target_OnSpawn);
            isApplied = true;
            OnApply();
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
