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
    [CreateAssetMenu(fileName = "PogoStickManifest", menuName = "Pogo/Cosmetics/PogoStickManifest")]
    public class PogoStickManifest : ScriptableObject, IDescriptorManifest<PogoStickDescriptor>
    {
        public PogoStickDescriptor[] Items;

        public void Add(PogoStickDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Items, descriptor);
        }

        public bool Contains(PogoStickDescriptor descriptor)
        {
            return Items.Contains(descriptor);
        }

        public void Remove(PogoStickDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Items, descriptor);
        }
    }
}
