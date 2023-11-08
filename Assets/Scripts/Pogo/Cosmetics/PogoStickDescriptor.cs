using Pogo.Collectibles;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "stick_", menuName = "Pogo/Cosmetics/PogoStick", order = 1)]
    public class PogoStickDescriptor : ScriptableObject
    {
        public enum UnlockTypes
        {
            AlwaysUnlocked,
            VendingMachine,
            Collectible
        }

        public UnlockTypes UnlockType;
        public EquipmentDescriptor Equipment;
        public bool AllowRecoloring;

        public string DisplayName;

        [HideInInspector]
        public CollectibleDescriptor Collectible;
    }
}
