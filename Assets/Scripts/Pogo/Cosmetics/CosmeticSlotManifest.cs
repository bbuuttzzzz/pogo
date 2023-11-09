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
    [CreateAssetMenu(fileName = "CosmeticSlotManifest", menuName = "Pogo/Cosmetics/CosmeticSlotManifest")]
    public class CosmeticSlotManifest : ScriptableObject, IDescriptorManifest<CosmeticDescriptor>
    {
        public CosmeticDescriptor Default;

        public CosmeticDescriptor[] Items;

        public CosmeticSlots Slot;

        public void Add(CosmeticDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Items, descriptor);
        }

        public bool Contains(CosmeticDescriptor descriptor)
        {
            return Items.Contains(descriptor);
        }

        public void Remove(CosmeticDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Items, descriptor);
        }

        public CosmeticDescriptor FindByKey(string key)
        {
            foreach(var item in Items)
            {
                if (item.Key == key) return item;
            }

            throw new KeyNotFoundException();
        }
    }
}
