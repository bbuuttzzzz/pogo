using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.ManifestPattern;

namespace Pogo.CustomMaps
{
    [CreateAssetMenu(fileName = "manifest_skymat", menuName = "Pogo/CustomMaps/SkyMaterialManifest")]
    public class SkyMaterialManifest : ScriptableObject, IDescriptorManifest<SkyMaterialDescriptor>
    {
        public SkyMaterialDescriptor[] Items;

        public bool Contains(SkyMaterialDescriptor descriptor)
        {
            return Items.Contains(descriptor);
        }

        public void Add(SkyMaterialDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Items, descriptor);
        }

        public void Remove(SkyMaterialDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Items, descriptor);
        }
    }
}
