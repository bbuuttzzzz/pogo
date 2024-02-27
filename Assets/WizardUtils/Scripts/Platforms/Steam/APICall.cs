using Pogo.CustomMaps.Steam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Platforms.Steam
{
    public class APICall<TSteamCallResult>
    {
        private string Name;
        private float LastCallTime;
        private CallResult<TSteamCallResult> CallResult;

        private Action<APICallResult<TSteamCallResult>> Callback;

        public APICall(string name)
        {
            Name = name;
            CallResult = new CallResult<TSteamCallResult>(CallResult_Receive);
        }

        private void CallResult_Receive(TSteamCallResult param, bool bIOFailure)
        {
            if (Callback == null)
            {
                throw new InvalidOperationException($"Missing Cached Parameters for APICall");
            }

            Callback.Invoke(new APICallResult<TSteamCallResult>()
            {
                CallResult = param,
                IOFailure = bIOFailure,
            });
            Callback = null;
        }

        public void Set(SteamAPICall_t call, Action<APICallResult<TSteamCallResult>> callback)
        {
            if (Callback != null)
            {
                Debug.LogWarning($"Concurrent API calls are unsupported. {Name} was called {Time.unscaledTime - LastCallTime} ago. throwing away old call!");
            }
            Callback = callback;
            LastCallTime = Time.unscaledTime;
            CallResult.Set(call);
        }
    }

    public struct APICallResult<TSteamCallResult>
    {
        public TSteamCallResult CallResult;
        public bool IOFailure;
    }
}
