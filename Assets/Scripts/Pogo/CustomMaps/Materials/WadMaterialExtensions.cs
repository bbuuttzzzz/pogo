using BSPImporter.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Materials
{
    public static class WadMaterialExtensions
    {
        public static string GetSurfaceType(this WadMaterial self)
        {
            return self.Metadata.GetValueOrDefault("surface");
        }
    }
}
