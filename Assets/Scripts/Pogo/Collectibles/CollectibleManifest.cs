using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.ManifestPattern;

namespace Pogo.Collectibles
{
    [CreateAssetMenu(fileName = "collectibleManifest", menuName = "Pogo/Collectibles/Manifest")]
    public class CollectibleManifest : ScriptableObject, IDescriptorManifest<CollectibleDescriptor>
    {
        public CollectibleDescriptor[] Collectibles;

        public void Add(CollectibleDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Collectibles, descriptor);
        }

        public bool Contains(CollectibleDescriptor descriptor)
        {
            return Collectibles.Contains(descriptor);
        }

        public void Remove(CollectibleDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Collectibles, descriptor);
        }
    }
}
