using Pogo;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointTrigger : Trigger
{
    public Transform RespawnPoint;

    public UnityEvent OnEnteredNotActivated;

    public override bool ColliderCanTrigger(Collider other)
    {
        if (WizardUtils.GameManager.GameInstanceIsValid())
        {
            bool success = PogoGameManager.PogoInstance.TryRegisterRespawnPoint(RespawnPoint);
            if (!success) OnEnteredNotActivated?.Invoke();
            return success;
        }
        else
            return false;
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