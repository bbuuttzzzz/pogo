using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Math;

namespace Pogo.Challenges
{
    [Serializable]
    public class Challenge
    {

        public enum ChallengeTypes
        {
            Create,
            Play
        }
        public ChallengeTypes ChallengeType;

        public LevelDescriptor Level;

        public Vector3 StartPoint => StartPointCm.ToVector3() / 100;
        public Vector3Short StartPointCm;

        int startYaw;
        public int StartYaw
        {
            get => startYaw;
            set
            {
                startYaw = ((value % 360) / 2) * 2;
            }
        }


        public Vector3 EndPoint => EndPointCm.ToVector3() / 100;
        public Vector3Short EndPointCm;

        public ushort LastAttemptTimeMS;
        public float LastAttemptTime => (LastAttemptTimeMS * 1f) / 1000;
        public float BestTime => (BestTimeMS * 1f) / 1000;

        public ushort BestTimeMS;

        public Quaternion StartRotation => Quaternion.Euler(0, StartYaw, 0);

        public Challenge(LevelDescriptor level, Transform start, Vector3 end)
        {
            ChallengeType = ChallengeTypes.Create;
            Level = level;
            StartPointCm = Vector3Short.FromVector3(start.position * 100);
            StartYaw = (int)start.rotation.eulerAngles.y;
            EndPointCm = Vector3Short.FromVector3(end * 100);
            BestTimeMS = ushort.MaxValue;
        }

        public Challenge()
        {

        }

        public float AttemptStartTime;
        public void StartAttempt()
        {
            AttemptStartTime = Time.time;
        }
        /// <summary>
        /// Finish counting the run's time
        /// </summary>
        /// <returns>True if the new time is faster</returns>
        public void FinishAttempt()
        {
            float time = Time.time - AttemptStartTime;
            LastAttemptTimeMS = (ushort)Mathf.RoundToInt((time * 1000));
            BestTimeMS = Math.Min(BestTimeMS, LastAttemptTimeMS);
        }
    }
}
