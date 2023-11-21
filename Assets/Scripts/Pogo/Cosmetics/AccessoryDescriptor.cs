using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "hat_", menuName = "Pogo/Cosmetics/AccessoryDescriptor", order = 1)]
    public class AccessoryDescriptor : CosmeticDescriptor
    {
        public EquipmentDescriptor Equipment;

        public override CosmeticSlots Slot => CosmeticSlots.Accessory;
    }
}
