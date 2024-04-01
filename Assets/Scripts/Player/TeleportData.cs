using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public struct TeleportData
    {
        public Vector3 Position;
        public float Yaw;
        public Vector3 Velocity;

        public TeleportData(Transform transform, Vector3 velocity) : this()
        {
            Position = transform.position;
            Yaw = transform.rotation.eulerAngles.y;
            Velocity = velocity;
        }

        public TeleportData(Vector3 position, float yaw, Vector3 velocity)
        {
            Position = position;
            Yaw = yaw;
            Velocity = velocity;
        }
    }
}
