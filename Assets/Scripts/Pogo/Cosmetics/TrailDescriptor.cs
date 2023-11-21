using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "trail_", menuName = "Pogo/Cosmetics/TrailDescriptor", order = 1)]
    public class TrailDescriptor : CosmeticDescriptor
    {
        public EquipmentDescriptor Equipment;

        public override CosmeticSlots Slot => CosmeticSlots.Trail;
    }
}
 