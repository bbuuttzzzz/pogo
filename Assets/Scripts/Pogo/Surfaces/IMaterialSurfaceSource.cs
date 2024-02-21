using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Surfaces
{
    public interface IMaterialSurfaceSource
    {
        /// <summary>
        /// Check for the material in this source.
        /// </summary>
        /// <param name="material"></param>
        /// <returns>the SurfaceConfig for this material, else NULL if unspecified</returns>
        public SurfaceConfig CheckMaterial(Material material);
    }
}
