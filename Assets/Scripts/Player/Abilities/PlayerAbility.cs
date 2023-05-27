using System;
using UnityEngine;

namespace Pogo.Abilities
{
    public abstract class PlayerAbility : MonoBehaviour
    {
        protected PlayerController Target;

        public enum CleanseTypes
        {
            CleanseOnDeath,
            CleanseOnTouch
        }
        public CleanseTypes CleanseType;

        public void Apply(PlayerController target)
        {
            Target = target;
            Target.OnTouch.AddListener(Target_OnTouch);
            Target.OnSpawn.AddListener(Target_OnSpawn);
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
            OnCleanse();
            Target.OnTouch.RemoveListener(Target_OnTouch);
            Target = null;
            Destroy(gameObject);
        }

        protected abstract void OnApply();
        protected abstract void OnCleanse();
    }
}
