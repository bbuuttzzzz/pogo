using System;
using UnityEngine;
using WizardPhysics.PhysicsTime;

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

        private void Awake()
        {
            PogoGameManager.PogoInstance.TimeManager.OnPhysicsUpdate.AddListener(OnPhysicsUpdate);
            PogoGameManager.PogoInstance.TimeManager.OnRenderUpdate.AddListener(OnRenderUpdate);
        }

        private void OnDestroy()
        {
            PogoGameManager.PogoInstance.TimeManager.OnPhysicsUpdate.RemoveListener(OnPhysicsUpdate);
            PogoGameManager.PogoInstance.TimeManager.OnRenderUpdate.RemoveListener(OnRenderUpdate);
        }


        private void OnPhysicsUpdate()
        {
            if (IsApplied)
            {
                AppliedPhysicsUpdate();
            }
        }

        private void OnRenderUpdate(RenderArgs arg0)
        {
            if (IsApplied)
            {
                AppliedRenderUpdate(arg0);
            }
        }

        public void Apply(PlayerController target)
        {
            Owner = target;
            Owner.OnBeforeApplyAbility?.Invoke(this);
            Owner.OnTouch.AddListener(Target_OnTouch);
            Owner.OnDie.AddListener(Target_OnSpawn);
            Owner.OnSpawn.AddListener(Target_OnSpawn);
            Owner.OnBeforeApplyAbility.AddListener(Target_OnBeforeApplyAbility);
            PogoGameManager.PogoInstance.OnQuitToMenu.AddListener(CleanseInstantly);
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

        public void Cleanse() => Cleanse(false);
        public void CleanseInstantly() => Cleanse(true);
        public void Cleanse(bool instant)
        {

            isApplied = false;
            OnCleanse();
            Owner.OnTouch.RemoveListener(Target_OnTouch);
            Owner.OnDie.RemoveListener(Target_OnSpawn);
            Owner.OnSpawn.RemoveListener(Target_OnSpawn);
            Owner.OnBeforeApplyAbility.RemoveListener(Target_OnBeforeApplyAbility);
            PogoGameManager.PogoInstance.OnQuitToMenu.RemoveListener(CleanseInstantly);

            Owner = null;
            if (instant)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject, DestroyDelay);
            }
        }

        protected virtual void Update()
        {
        }

        protected abstract void OnApply();
        protected abstract void OnCleanse();
        protected abstract void AppliedPhysicsUpdate();
        protected abstract void AppliedRenderUpdate(RenderArgs arg0);
    }
}
