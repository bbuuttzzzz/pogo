using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Challenges
{
    [System.Serializable]
    public struct EncodeChallengeResult
    {
        public string Code;
        public FailReasons FailReason;
        [System.Serializable]
        public enum FailReasons
        {
            None,
            MissingShareIndex
        }
        public readonly bool Success => FailReason == FailReasons.None;

        public static EncodeChallengeResult NewSuccess(string code) => new EncodeChallengeResult()
        {
            Code = code,
            FailReason = FailReasons.None
        };

        public static EncodeChallengeResult NewFailure(FailReasons failReason) => new EncodeChallengeResult()
        {
            Code = null,
            FailReason = failReason
        };
    }
}
