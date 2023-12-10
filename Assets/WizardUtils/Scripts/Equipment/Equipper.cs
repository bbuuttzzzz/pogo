using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace WizardUtils.Equipment
{
    public class Equipper : MonoBehaviour
    {
        [HideInInspector]
        public EquipmentSlot[] EquipmentSlots;

        public UnityEvent<EquipmentSlot> OnEquip;

        private void Awake()
        {
            ApplyAll();
        }

        #region Applying
        private void ApplyAll()
        {
            foreach(EquipmentSlot slot in EquipmentSlots)
            {
                ApplySlot(slot);
            }
        }

        private void ApplySlot(EquipmentSlot slot)
        {
            if (slot.Equipment == null) return;
            if (slot.ObjectInstance != null) return;

#if UNITY_EDITOR
            if (slot.Equipment.SlotType != slot.EquipmentType)
            {
                Debug.LogError($"EquipmentSlot mismatch. Equipment SlotType {slot.Equipment.SlotType} != {slot.EquipmentType}");
            }
#endif
            var result = Instantiate(slot.Equipment.Prefab, slot.PrefabInstantiationParent);
            slot.ObjectInstance = result;

            var stateListener = result.GetComponent<IEquipmentRoot>();
            if (stateListener != null)
            {
                stateListener.OnEquipped();
            }
        }

        private void DeApplySlot(EquipmentSlot slot)
        {
            if (slot.ObjectInstance != null)
            {
                var stateListener = slot.ObjectInstance.GetComponent<IEquipmentRoot>();
                if (stateListener != null)
                {
                    stateListener.OnUnequipped();
                }

                Destroy(slot.ObjectInstance);
                slot.ObjectInstance = null;
            }
        }

#if UNITY_EDITOR
        public void DeApplySlotInEditor(EquipmentSlot slot)
        {
            if (slot.ObjectInstance != null)
            {
                if (Application.isPlaying)
                    Destroy(slot.ObjectInstance);
                else
                {
                    DestroyImmediate(slot.ObjectInstance);
                }

                slot.ObjectInstance = null;
            }
        }

        public void ApplySlotInEditor(EquipmentSlot slot)
        {
            if (slot.Equipment == null) return;

            if (slot.Equipment.SlotType != slot.EquipmentType)
            {
                Debug.LogError($"EquipmentSlot mismatch. Equipment SlotType {slot.Equipment.SlotType} != {slot.EquipmentType}");
            }

            if (Application.isPlaying)
            {
                var result = Instantiate(slot.Equipment.Prefab, slot.PrefabInstantiationParent);
                slot.ObjectInstance = result;
            }
            else
            {
                var result = UnityEditor.PrefabUtility.InstantiatePrefab(slot.Equipment.Prefab, slot.PrefabInstantiationParent);
                slot.ObjectInstance = result as GameObject;
            }
        }
#endif
        #endregion


        #region Equipping
        public void Equip(EquipmentDescriptor equipment)
        {
            var slot = FindSlot(equipment.SlotType);

            if (slot == null)
            {
                Debug.LogWarning($"Tried to equip equipment {equipment} without its slot {equipment.SlotType}");
                return;
            }

            if (slot.Equipment != null)
            {
                DeApplySlot(slot);
            }

            slot.Equipment = equipment;
            ApplySlot(slot);
            OnEquip?.Invoke(slot);
        }

        public void UnEquip(EquipmentSlot slot)
        {
            DeApplySlot(slot);
            slot.Equipment = null;
        }

#if UNITY_EDITOR
        public void EquipInEditor(EquipmentDescriptor equipment)
        {
            var slot = FindSlot(equipment.SlotType);

            if (slot.Equipment != null && slot.ObjectInstance != null)
            {
                DeApplySlotInEditor(slot);
            }

            slot.Equipment = equipment;
            ApplySlotInEditor(slot);
        }

        public void UnEquipInEditor(EquipmentSlot slot)
        {
            DeApplySlotInEditor(slot);
            slot.Equipment = null;
        }
#endif
        #endregion

        #region Slot Helpers
        public EquipmentSlot FindSlot(EquipmentTypeDescriptor slotType)
        {
            foreach (var slot in EquipmentSlots)
            {
                if (slot.EquipmentType == slotType)
                {
                    return slot;
                }
            }

            return null;
        }

        public bool HasSlot(EquipmentTypeDescriptor slotType)
        {
            foreach (var slot in EquipmentSlots)
            {
                if (slot.EquipmentType == slotType)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
