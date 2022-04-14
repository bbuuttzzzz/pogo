using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Math;

namespace Pogo.Challenges
{
    public class Challenge
    {
        public LevelDescriptor Level;

        public Vector3 StartPoint => startPointCm.ToVector3() / 100;
        public Vector3Short StartPointCm => startPointCm;
        public Vector3Short startPointCm;

        int startYaw;
        public int StartYaw
        {
            get => startYaw;
            set
            {
                startYaw = ((value % 360) / 2) * 2;
            }
        }


        public Vector3 EndPoint => endPointCm.ToVector3() / 100;
        public Vector3Short EndPointC => endPointCm;
        public Vector3Short endPointCm;

        public Challenge(LevelDescriptor level, Transform start, Vector3 end)
        {
            Level = level;
            startPointCm = Vector3Short.FromVector3(start.position * 100);
            StartYaw = (int)start.rotation.eulerAngles.y;
            endPointCm = Vector3Short.FromVector3(end * 100);
        }

        public Challenge()
        {

        }
    }
}
