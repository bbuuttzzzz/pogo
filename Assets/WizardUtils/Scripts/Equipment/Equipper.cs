using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils.Equipment
{
    public class Equipper : MonoBehaviour
    {
        public EquipmentSlotManifest Manifest;
        public EquipmentSlot[] StartingEquipment;

        [HideInInspector]
        public List<EquipmentSlot> Equipment;

        public UnityEvent<EquipmentSlot> OnEquip;

        public bool EquipAllOnAwake = true;

        public void EquipAll()
        {
            foreach(EquipmentSlot slot in StartingEquipment)
            {
                Equip(slot);
            }
        }

        public void Equip(EquipmentSlot slot)
        {
#if UNITY_EDITOR
            if (!VerifySlot(slot))
            {
                Debug.LogError($"EquipmentSlot mismatch. no slot {slot.Slot} in Manifest {Manifest}");
            }
#endif

            var result = Instantiate(slot.Equipment.Prefab, slot.PrefabInstantiationParent);
            slot.ObjectInstance = result;
        }

        private bool VerifySlot(EquipmentSlot slot)
        {
            foreach (var verifyingSlotDescriptor in Manifest.Slots)
            {
                if (verifyingSlotDescriptor == slot.Slot)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
