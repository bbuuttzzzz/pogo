using Newtonsoft.Json;
using Pogo.Collectibles;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct CollectibleUnlockData
    {
        public string key;
        public bool isUnlocked;
    }
}