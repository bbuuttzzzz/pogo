using UnityEngine;

namespace Pogo
{
    public class SkipPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            //resources.Load every frame to draw this btw LOL!!!
            Mesh arrowMesh = Resources.Load<Mesh>("Models/enterIndicator");

            Gizmos.color = Color.yellow;
            Gizmos.DrawMesh(arrowMesh, transform.position, transform.rotation, Vector3.one * 0.25f);
        }
    }
}
