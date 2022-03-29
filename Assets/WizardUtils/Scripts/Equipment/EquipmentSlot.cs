using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Equipment
{
    [Serializable]
    public class EquipmentSlot
    {
        public EquipmentSlotDescriptor Slot;
        public EquipmentDescriptor Equipment;
        public Transform PrefabInstantiationParent;
        [NonSerialized]
        public GameObject ObjectInstance;
    }
}
