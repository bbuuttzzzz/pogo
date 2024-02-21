using UnityEngine;

namespace Pogo.Surfaces
{
    public interface ISurfaceConfigOverride
    {
        public SurfaceConfig CheckSurface(RaycastHit hitInfo);
    }
}
