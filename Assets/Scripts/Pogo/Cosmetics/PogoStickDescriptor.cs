using Pogo.Collectibles;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "stick_", menuName = "Pogo/Cosmetics/PogoStick", order = 1)]
    public class PogoStickDescriptor : CosmeticDescriptor
    {
        public EquipmentDescriptor Equipment;
    }
}
