using UnityEngine;

namespace Pogo
{
    public class RespawnPoint : MonoBehaviour
    {
        public bool EnabledInHardMode;
        private void OnDrawGizmos()
        {
                //resources.Load every frame to draw this btw LOL!!!
                Mesh arrowMesh = Resources.Load<Mesh>("Models/enterIndicator");

                Gizmos.color = Color.green;
                Gizmos.DrawMesh(arrowMesh, transform.position, transform.rotation, Vector3.one);
        }
    }
}
