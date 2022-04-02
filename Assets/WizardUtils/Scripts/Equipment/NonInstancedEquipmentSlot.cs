using System;
using UnityEngine;
namespace WizardUtils.Equipment
{
    [Serializable]
    public class NonInstancedEquipmentSlot
    {
        public EquipmentTypeDescriptor EquipmentType;
        public EquipmentDescriptor Equipment;

        public NonInstancedEquipmentSlot(EquipmentTypeDescriptor equipmentType)
        {
            EquipmentType = equipmentType;
        }
    }
}
