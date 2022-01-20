using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointTrigger : MonoBehaviour
{
    public Transform RespawnPoint;

    public UnityEvent OnActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.TryRegisterRespawnPoint(RespawnPoint))
        {
            OnActivated.Invoke();
        }
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