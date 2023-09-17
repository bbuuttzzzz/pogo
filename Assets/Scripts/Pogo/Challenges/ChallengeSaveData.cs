using Newtonsoft.Json;
using Pogo.Collectibles;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct ChallengeSaveData
    {
        public string key;
        public int bestTimeMS;
    }
}