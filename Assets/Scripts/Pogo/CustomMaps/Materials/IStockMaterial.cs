using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    public interface IStockMaterial
    {
        public string Name { get; }
        public Material Material { get; }
    }
}
