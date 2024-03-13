using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Surfaces
{
    public class SurfaceSource : IMaterialSurfaceSource
    {
        protected Dictionary<Material, SurfaceConfig> surfacePropertiesDict;

        public SurfaceSource()
        {
            surfacePropertiesDict = new Dictionary<Material, SurfaceConfig>();
        }

        public void RegisterMaterial(Material material, SurfaceConfig config)
        {
            try
            {
                surfacePropertiesDict.Add(material, config);
            }
            catch (ArgumentException e)
            {
                // throw a pretty error for duplicate materials
                var existingSurfaceConfigName = surfacePropertiesDict[material].name;
                throw new ArgumentException($"Duplicate Surface Definition for material {material.name}: {existingSurfaceConfigName} & {config.name}", e);
            }
        }

        public virtual SurfaceConfig CheckMaterial(Material material)
        {
            return surfacePropertiesDict.GetValueOrDefault(material, null);
        }
    }
}
