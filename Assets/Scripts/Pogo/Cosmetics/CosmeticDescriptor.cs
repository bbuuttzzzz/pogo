using Pogo.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "_", menuName = "Pogo/Cosmetics/CosmeticDescriptor", order = 1)]
    public class CosmeticDescriptor : ScriptableObject
    {
        public enum UnlockTypes
        {
            AlwaysUnlocked,
            VendingMachine,
            Collectible
        }

        public UnlockTypes UnlockType;
        public string DisplayName;
        public string Key => name;
        public Sprite Icon;
        public string OverrideUnlockText;

        public CosmeticSlots Slot;
        public EquipmentDescriptor Equipment;

        public bool AllowRecoloring;

        [HideInInspector]
        public CollectibleDescriptor Collectible;
    }
}
