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
    public class CosmeticManifest : ScriptableObject, IDescriptorManifest<CosmeticDescriptor>
    {
        public CosmeticDescriptor[] Items;

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
    }
}
