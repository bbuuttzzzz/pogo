using Pogo.Levels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.Tools
{
    public class LevelOriginDefiner : MonoBehaviour
    {
        public LevelDescriptor Level;

        private void OnDrawGizmosSelected()
        {
            if (Level == null) return;

            if (Level.ShareOrigin == transform.position)
            {
                Gizmos.color = Color.green;
                DrawChallengeBoundingBox(transform.position);
            }
            else
            {
                Gizmos.color = Color.red;
                DrawChallengeBoundingBox(Level.ShareOrigin);
                Gizmos.color = Color.white;
                DrawChallengeBoundingBox(transform.position);
            }
        }

        private void DrawChallengeBoundingBox(Vector3 center)
        {
            Gizmos.DrawWireCube(center, Vector3.one * (short.MaxValue * 2 / 100));
        }
    }
}
