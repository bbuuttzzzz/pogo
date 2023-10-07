using Pogo.Saving;
using System.Collections.Generic;

public static class AntiAOTStripping
{
    public static void DoNotCallMe()
    {
        List<CollectibleUnlockData> data = new List<CollectibleUnlockData>();
        data.Add(new CollectibleUnlockData());
        data.Add(data[0]);
    }
}