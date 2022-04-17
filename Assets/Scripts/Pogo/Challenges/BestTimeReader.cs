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
        public UnityEvent<string> OnReadBestTimeText;

        public void Read(Challenge challenge)
        {
            string text;
            if (challenge.BestTimeMS == ushort.MaxValue)
            {
                text = null;
            }
            else
            {
                var timeSpan = TimeSpan.FromSeconds(challenge.BestTime);
                text = timeSpan.ToString("s\\.fff");
            }

            OnReadBestTimeText?.Invoke(text);
        }
    }
}
