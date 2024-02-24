using Pogo.CustomMaps.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.ManifestPattern;

namespace Pogo.CustomMaps.Materials
{
    [CreateAssetMenu(fileName = "manifest_cmat", menuName = "Pogo/CustomMaps/FillableShaderManifest")]
    public class FillableShaderManifest : ScriptableObject, IDescriptorManifest<FillableShaderDescriptor>
    {
        public FillableShaderDescriptor[] Items;

        public bool Contains(FillableShaderDescriptor descriptor)
        {
            return Items.Contains(descriptor);
        }

        public void Add(FillableShaderDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Items, descriptor);
        }

        public void Remove(FillableShaderDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Items, descriptor);
        }
    }
}
