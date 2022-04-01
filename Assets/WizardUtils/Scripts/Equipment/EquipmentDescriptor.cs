using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Equipment
{
    [CreateAssetMenu(fileName = "EquipmentDescriptor", menuName = "WizardUtils/Equipment/EquipmentDescriptor", order = 1)]

    public class EquipmentDescriptor : ScriptableObject
    {
        public EquipmentTypeDescriptor SlotType;
        public GameObject Prefab;

        public string DisplayName;
        public string Description;
    }
}
