using UnityEngine;

namespace Pogo.Surfaces
{
    public class AssetSurfaceSource : SurfaceSource
    {
        private bool LogMissingMaterials;
        public AssetSurfaceSource(bool logMissingMaterials = true) : base()
        {
            LogMissingMaterials = logMissingMaterials;
            var surfaceConfigs = Resources.LoadAll<SurfaceConfig>("Surfaces");
            foreach (var config in surfaceConfigs)
            {
                foreach (var material in config.Materials)
                {
                    RegisterMaterial(material, config);
                }
            }
        }

        public override SurfaceConfig CheckMaterial(Material material)
        {
            var surface = base.CheckMaterial(material);
            if (surface == null && LogMissingMaterials)
            {
                Debug.LogWarning($"Missing surfaceConfig for material {material}", material);
            }
            return surface;
        }
    }
}
