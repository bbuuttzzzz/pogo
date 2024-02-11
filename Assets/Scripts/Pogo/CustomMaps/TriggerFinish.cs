using UnityEngine;

namespace Pogo.CustomMaps
{
    public class TriggerFinish : GeneratedCheckpoint
    {
        public Transform DefaultRespawnPoint;

        private void Start()
        {
            Collider col = GetComponent<Collider>();
            DefaultRespawnPoint.transform.position = col.bounds.center;
        }
    }
}
