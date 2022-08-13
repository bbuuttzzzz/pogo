using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Math;
using WizardUtils.Saving;

namespace Pogo.Challenges
{
    [Serializable]
    public class Challenge
    {

        public enum ChallengeTypes
        {
            Create,
            PlayCustom,
            PlayDeveloper
        }

        public const int WORST_TIME = 60_000;
        public ChallengeTypes ChallengeType;

        public LevelDescriptor Level;
        public DeveloperChallenge DeveloperChallenge;

        public Vector3 StartPoint => StartPointCm.ToVector3() / 100;
        public Vector3Short StartPointCm;

        [SerializeField]
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

        public float PersonalBestTime => (PersonalBestTimeMS * 1f) / 1000;
        public ushort PersonalBestTimeMS;

        public Quaternion StartRotation => Quaternion.Euler(0, StartYaw, 0);

        public Challenge(LevelDescriptor level, Transform start, Vector3 end) : this()
        {
            ChallengeType = ChallengeTypes.Create;
            Level = level;
            StartPointCm = Vector3Short.FromVector3(start.position * 100);
            StartYaw = (int)start.rotation.eulerAngles.y;
            EndPointCm = Vector3Short.FromVector3(end * 100);
        }

        public Challenge()
        {
            BestTimeMS = WORST_TIME;
            PersonalBestTimeMS = ushort.MaxValue;
            LastAttemptTimeMS = ushort.MaxValue;
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
        public ChallengeAttemptData FinishAttempt()
        {
            var data = new ChallengeAttemptData(PersonalBestTimeMS);
            float time = Time.time - AttemptStartTime;
            LastAttemptTimeMS = (ushort)Mathf.RoundToInt((time * 1000));

            if (LastAttemptTimeMS < PersonalBestTimeMS)
            {
                data.NewTimeBetter = true;
                if (ChallengeType == ChallengeTypes.PlayDeveloper)
                {
                    if (LastAttemptTimeMS < BestTimeMS && DeveloperChallenge.BestTimeMS > BestTimeMS)
                    {
                        data.GoldMedalEarned = true;
                    }

                    DeveloperChallenge.BestTimeMS = LastAttemptTimeMS;
                }

                if (PersonalBestTimeMS == WORST_TIME)
                {
                    data.FirstClear = true;
                }
                PersonalBestTimeMS = LastAttemptTimeMS;
                
            }

            BestTimeMS = Math.Min(BestTimeMS, LastAttemptTimeMS);
            data.NewTimeMS = BestTimeMS;

            return data;
        }

        public struct ChallengeAttemptData
        {
            public ushort OldTimeMS;
            public ushort NewTimeMS;
            public bool FirstClear;
            public bool GoldMedalEarned;
            public bool NewTimeBetter;

            public ChallengeAttemptData(ushort oldTimeMS) : this()
            {
                OldTimeMS = oldTimeMS;
            }
        }
    }
}
