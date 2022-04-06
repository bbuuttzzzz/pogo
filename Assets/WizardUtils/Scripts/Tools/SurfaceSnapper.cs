using UnityEngine;
using WizardUtils.Extensions;

namespace WizardUtils.Tools
{
    public class SurfaceSnapper : MonoBehaviour
    {
        public Transform Target;
        public float MaxDistance = 1;

        [HideInInspector]
        public int mask;

        Vector3 surfaceForward => transform.rotation * Vector3.down;

        public void Snap()
        {

        }

        private void OnValidate()
        {
            mask = LAYERMASK.MaskForLayer(gameObject.layer);
        }

        bool CheckForSurface(out RaycastHit hitInfo)
        {
            Ray ray = new Ray(transform.position, surfaceForward);
            bool didHit = Physics.Raycast(
                ray: ray,
                maxDistance: MaxDistance,
                layerMask: mask,
                hitInfo: out hitInfo,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore);

            return didHit;
        }

        private void OnDrawGizmosSelected()
        {
            if (CheckForSurface(out RaycastHit hitInfo))
            {
                Gizmos.color = new Color(1, .85f, 0);
                Gizmos.DrawRay(transform.position, surfaceForward * hitInfo.distance);

                var meshFilter = Target.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    GizmosHelper.DrawWireMeshFlush(meshFilter.sharedMesh, hitInfo.point, hitInfo.normal, Target.localRotation, Target.lossyScale);   
                }
                else
                {
                    const float size = 0.3f;
                    GizmosHelper.DrawCubeFlush(hitInfo.point, hitInfo.normal, new Vector3(size, size, size));
                }
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, surfaceForward * MaxDistance);
                
            }
        }
    }
}
