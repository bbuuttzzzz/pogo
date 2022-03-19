using Pogo;
using UnityEngine;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour
{
    public Transform Target;

    public void TeleportPlayer()
    {
        var player = PogoGameManager.PogoInstance?.Player;
        player.TeleportTo(Target);
    }

    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            //resources.Load every frame to draw this btw LOL!!!
            Mesh arrowMesh = Resources.Load<Mesh>("Models/enterIndicator");

            Gizmos.color = Color.red;
            Gizmos.DrawMesh(arrowMesh, Target.position, Target.rotation, Vector3.one);

            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}