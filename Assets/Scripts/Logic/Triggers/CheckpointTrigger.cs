using Pogo;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointTrigger : Trigger
{
    public Transform RespawnPoint;

    public override bool ColliderCanTrigger(Collider other)
    {
        return PogoGameManager.TryRegisterRespawnPoint(RespawnPoint);
    }

    private void OnDrawGizmos()
    {
        if (RespawnPoint != null)
        {
            //resources.Load every frame to draw this btw LOL!!!
            Mesh arrowMesh = Resources.Load<Mesh>("Models/enterIndicator");

            Gizmos.color = Color.green;
            Gizmos.DrawMesh(arrowMesh, RespawnPoint.position, RespawnPoint.rotation, Vector3.one);
        }
    }
}