using Pogo.Collectibles;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "stick_", menuName = "Pogo/Cosmetics/PogoStickDescriptor", order = 1)]
    public class PogoStickDescriptor : CosmeticDescriptor
    {
        public EquipmentDescriptor Equipment;

        public override CosmeticSlots Slot => CosmeticSlots.PogoStick;
    }
}
 