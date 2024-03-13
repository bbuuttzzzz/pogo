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
    [CreateAssetMenu(fileName = "manifest_stockmat", menuName = "Pogo/CustomMaps/StockMaterialManifest")]
    public class StockMaterialManifest : ScriptableObject, IDescriptorManifest<StockMaterialDescriptor>
    {
        public StockMaterialDescriptor[] Items;

        public bool Contains(StockMaterialDescriptor descriptor)
        {
            return Items.Contains(descriptor);
        }

        public void Add(StockMaterialDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Items, descriptor);
        }

        public void Remove(StockMaterialDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Items, descriptor);
        }
    }
}
