using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Challenges
{
    public class BestTimeReader : MonoBehaviour
    {
        public enum BestTimeType
        {
            Personal,
            World
        }
        public BestTimeType ReadType;

        public UnityEvent<string> OnReadBestTimeText;

        public void Read(Challenge challenge)
        {
            if (challenge == null) return;

            string text;
            ushort bestTimeMS = ReadType switch
            {
                BestTimeType.Personal => challenge.PersonalBestTimeMS,
                BestTimeType.World => challenge.BestTimeMS,
                _ => throw new NotImplementedException()
            };

            if (bestTimeMS == ushort.MaxValue)
            {
                text = null;
            }
            else
            {
                float bestTime = ReadType switch
                {
                    BestTimeType.Personal => challenge.PersonalBestTime,
                    BestTimeType.World => challenge.BestTime,
                    _ => throw new NotImplementedException()
                };
                text = bestTime.ToString("N3");
            }

            OnReadBestTimeText?.Invoke(text);
        }
    }
}
