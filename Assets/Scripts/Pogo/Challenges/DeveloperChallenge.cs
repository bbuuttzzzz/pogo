using Pogo;
using Pogo.Challenges;
using System.Collections;
using UnityEngine;
using WizardUtils;
using WizardUtils.Saving;

[CreateAssetMenu(fileName = "DeveloperChallenge", menuName = "Pogo/Challenges/Challenge", order = 1)]
public class DeveloperChallenge : ScriptableObject
{
    public string DisplayName;
    public string CreatorName;
    public Challenge Challenge;

    public string Key => name;

    public int BestTimeMS
    {
        get => PogoGameManager.PogoInstance.CurrentGlobalDataTracker.GetChallenge(Key).bestTimeMS;
        set
        {
            var data = PogoGameManager.PogoInstance.CurrentGlobalDataTracker.GetChallenge(Key);
            data.bestTimeMS = value;
            PogoGameManager.PogoInstance.CurrentGlobalDataTracker.SetChallenge(data);
        }
    }

    public float BestTime => (float)BestTimeMS / 1000;
}