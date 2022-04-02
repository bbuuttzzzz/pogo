using System;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo
{
    [RequireComponent(typeof(Equipper))]
    public class GameEquipmentListener : MonoBehaviour
    {
        public EquipmentTypeDescriptor[] AllowedTypes;
        Equipper equipper;

        public bool EquipOnAwake;

        private void Start()
        {
            equipper = GetComponent<Equipper>();
            if(PogoGameManager.GameInstanceIsValid())
            {
                PogoGameManager.PogoInstance.OnEquip?.AddListener(equipSlot);

                if (EquipOnAwake)
                {
                    foreach (var slot in PogoGameManager.PogoInstance.Loadout)
                    {
                        equipSlot(slot);
                    }
                }
            }
        }

        private void equipSlot(NonInstancedEquipmentSlot slot)
        {
            if (slot.Equipment == null) return;

            foreach(EquipmentTypeDescriptor type in AllowedTypes)
            {
                if (slot.EquipmentType == type)
                {
                    equipper.Equip(slot.Equipment);
                    return;
                }
            }
        }
    }
}
