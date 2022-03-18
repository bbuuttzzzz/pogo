using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class ChapterStartPoint : MonoBehaviour
    {
        public ChapterDescriptor Chapter;

        public UnityEvent OnLoaded;

        private void OnDrawGizmosSelected()
        {
            //resources.Load every frame to draw this btw LOL!!!
            Mesh arrowMesh = Resources.Load<Mesh>("Models/enterIndicator");

            Gizmos.color = Color.blue;
            Gizmos.DrawWireMesh(arrowMesh, transform.position, transform.rotation, Vector3.one);
        }
    }
}
