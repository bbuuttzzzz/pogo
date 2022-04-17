using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class ChallengePickupController : MonoBehaviour
    {
        public UnityEvent OnReset;

        public void Reset()
        {
            OnReset?.Invoke();
        }
    }
}
