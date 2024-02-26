using Pogo.CustomMaps.Steam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforms.Steam
{
    public class APICall<TSteamCallResult>
    {
        private CallResult<TSteamCallResult> CallResult;

        private Action<APICallResult<TSteamCallResult>> Callback;

        public APICall()
        {
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
        }

        public void Set(SteamAPICall_t call, Action<APICallResult<TSteamCallResult>> callback)
        {
            CallResult.Set(call);
        }
    }

    public struct APICallResult<TSteamCallResult>
    {
        public TSteamCallResult CallResult;
        public bool IOFailure;
    }
}
