using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.ManifestPattern;

namespace Pogo.Cosmetics
{
    [CreateAssetMenu(fileName = "CosmeticManifest", menuName = "Pogo/Cosmetics/CosmeticManifest")]
    public class CosmeticManifest : ScriptableObject, IDescriptorManifest<CosmeticSlotManifest>
    {
        public CosmeticSlotManifest[] Slots;

        public void Add(CosmeticSlotManifest descriptor)
        {
            ArrayHelper.InsertAndResize(ref Slots, descriptor);
        }

        public bool Contains(CosmeticSlotManifest descriptor)
        {
            return Slots.Contains(descriptor);
        }

        public void Remove(CosmeticSlotManifest descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Slots, descriptor);
        }
    }
}
