using Newtonsoft.Json;
using PoseClipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Cosmetics
{
    public class TrailIconGenerator : MonoBehaviour
    {
        public Vector3[] Positions;

        [ContextMenu("Set Child Trails Now")]
        public void SetupChildTrails()
        {
            var trails = GetComponentsInChildren<TrailRenderer>();
            foreach (var trail in trails)
            {
                trail.Clear();
                trail.AddPositions(Positions);
            }
        }
    }
}
