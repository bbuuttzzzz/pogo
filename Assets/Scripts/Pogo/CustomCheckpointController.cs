using Pogo;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils.Extensions;

namespace Assets.Scripts.Pogo
{
    public class CustomCheckpointController : MonoBehaviour
    {
        public UnityEvent OnPlaced;

        [HideInInspector]
        public int layermask;

        public float Radius;

        public float Height;

        public bool Place(Vector3 targetPosition)
        {
            if (CanPlace(targetPosition, out Vector3 finalPosition))
            {
                transform.position = finalPosition;
                OnPlaced?.Invoke();
                return true;
            }
            return false;
        }

        public bool CanPlace(Vector3 targetPosition, out Vector3 finalPosition)
        {
            if (enoughRoomAtLocation(targetPosition))
            {
                if (enoughRoomBelow(targetPosition, out RaycastHit belowHit))
                {
                    finalPosition = targetPosition;
                    return true;
                }
                else
                {
                    if (enoughRoomAbove(targetPosition, belowHit, out RaycastHit aboveHit))
                    {
                        Vector3 bottomPoint = targetPosition + Vector3.down * belowHit.distance;
                        finalPosition = bottomPoint + Vector3.up * Height;
                        return true;
                    }
                    else
                    {
                        finalPosition = targetPosition;
                        return false;
                    }
                }
            }
            else
            {
                finalPosition = targetPosition;
                return false;
            }
        }

        bool enoughRoomAtLocation(Vector3 newPosition)
        {
            return !Physics.CheckSphere(newPosition, Radius, layermask, QueryTriggerInteraction.Ignore);
        }

        bool enoughRoomBelow(Vector3 newPosition, out RaycastHit hitInfo)
        {
            return check(newPosition, Vector3.down, Height, out hitInfo);
        }

        bool enoughRoomAbove(Vector3 newPosition, RaycastHit belowCheckHitInfo, out RaycastHit aboveCheckHitInfo)
        {
            var distance = Height - belowCheckHitInfo.distance;
            return check(newPosition, Vector3.up, distance, out aboveCheckHitInfo);
        }

        bool check(Vector3 newPosition, Vector3 direction, float distance, out RaycastHit hitInfo)
        {
            Ray ray = new Ray(newPosition, direction);
            return !Physics.SphereCast(ray, Radius, out hitInfo, distance, layermask, QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmosSelected()
        {
            if (enoughRoomAtLocation(transform.position))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, Radius);

                if (enoughRoomBelow(transform.position, out RaycastHit belowHit))
                {
                    Gizmos.color = Color.green;
                    Vector3 bottomPoint = transform.position + Vector3.down * Height;
                    Gizmos.DrawWireSphere(bottomPoint, Radius);
                    GizmosHelper.DrawSphereConnection(transform.position, bottomPoint, Radius, Radius);
                }
                else
                {
                    if (enoughRoomAbove(transform.position, belowHit, out RaycastHit aboveHit))
                    {
                        Gizmos.color = new Color(0.5f, 1, 0.5f);
                        Vector3 bottomPoint = transform.position + Vector3.down * belowHit.distance;
                        Vector3 topPoint = bottomPoint + Vector3.up * Height;
                        Gizmos.DrawWireSphere(bottomPoint, Radius);
                        Gizmos.DrawWireSphere(topPoint, Radius);
                        GizmosHelper.DrawSphereConnection(bottomPoint, topPoint, Radius, Radius);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Vector3 center1 = transform.position + Vector3.down * belowHit.distance;
                        Vector3 center2 = transform.position + Vector3.up * aboveHit.distance;
                        Gizmos.DrawWireSphere(center1, Radius);
                        Gizmos.DrawWireSphere(center2, Radius);
                        GizmosHelper.DrawSphereConnection(center1, center2, Radius, Radius);
                    }
                }
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, Radius);
            }
        }
        
        private void OnValidate()
        {
            layermask = LAYERMASK.MaskForLayer(gameObject.layer);
        }

    }
}
