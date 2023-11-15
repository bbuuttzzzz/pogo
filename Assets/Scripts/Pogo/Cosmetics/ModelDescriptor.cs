using Pogo.Collectibles;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "model_", menuName = "Pogo/Cosmetics/ModelDescriptor", order = 1)]
    public class ModelDescriptor : CosmeticDescriptor
    {
        public EquipmentDescriptor Equipment;

        public override CosmeticSlots Slot => CosmeticSlots.Model;
    }
}
 