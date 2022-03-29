using System.Collections;
using UnityEngine;

namespace WizardUtils.Equipment
{
    [CreateAssetMenu(fileName = "EquipmentSlotManifest", menuName = "WizardUtils/Equipment/EquipmentSlotManifest", order = 1)]
    public class EquipmentSlotManifest : ScriptableObject
    {
        public EquipmentSlotDescriptor[] Slots;
    }
}