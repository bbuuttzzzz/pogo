using Pogo.Abilities;
using System;
using UnityEngine;

namespace Pogo
{
    public class AbilityZone : MonoBehaviour
    {
        public AbilityDescriptor Ability;

        private PlayerAbility cachedAbilityInstance;

        public void Apply(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player == null) return;
            if (cachedAbilityInstance != null)
            {
                cachedAbilityInstance.CleanseInstantly();
            }

            var abilityObject = Instantiate(Ability.Prefab, player.RenderTransform);
            cachedAbilityInstance = abilityObject.GetComponent<PlayerAbility>();
            cachedAbilityInstance.Apply(player);
        }

        public void Remove(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player == null) return;
            if (cachedAbilityInstance == null || !cachedAbilityInstance.IsApplied)
            {
                return;
            }

            cachedAbilityInstance.Cleanse();
            cachedAbilityInstance = null;
        }
    }
}
