using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    [CreateAssetMenu(fileName = "stockmat_", menuName = "Pogo/CustomMaps/StockMaterialDescriptor")]
    public class StockMaterialDescriptor : ScriptableObject, IStockMaterial
    {
        public string Name;
        public Material Material;

        string IStockMaterial.Name => Name;
        Material IStockMaterial.Material => Material;
    }
}
