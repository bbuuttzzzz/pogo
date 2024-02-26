using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforms.Steam
{
    public class APICall<TSteamCallResult, TResult>
    {
        private RawAPICall<TSteamCallResult> RawAPICall;

        private Func<APICallResult<TSteamCallResult>, TResult> ProcessFunc;

        public APICall(Func<APICallResult<TSteamCallResult>, TResult> process)
        {
            RawAPICall = new RawAPICall<TSteamCallResult>();
            ProcessFunc = process;
        }

        public void Set(SteamAPICall_t call, Action<TResult> callback)
        {
            RawAPICall.Set(call, (result) =>
            {
                var processedResult = ProcessFunc(result);
                callback(processedResult);
            });
        }
    }
}
