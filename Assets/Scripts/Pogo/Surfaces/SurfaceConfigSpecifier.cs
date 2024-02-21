using Pogo.Surfaces;
using UnityEngine;

namespace Pogo
{
    public class SurfaceConfigSpecifier : MonoBehaviour, ISurfaceConfigOverride
    {
        public SurfaceConfig SurfaceConfig;

        public SurfaceConfig CheckSurface(RaycastHit hitInfo) => SurfaceConfig;
    }
}
