using Pogo;
using Pogo.Challenges;
using System.Collections;
using UnityEngine;
using WizardUtils;
using WizardUtils.Saving;

[CreateAssetMenu(fileName = "DeveloperChallenge", menuName = "Pogo/DeveloperChallenge", order = 1)]
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

    public int BestTimeMS_Legacy
    {
        get
        {
            if (int.TryParse(PlayerTimeSaveValue_Legacy.CurrentValue, out int result))
            {
                return result;
            }
            return Challenge.WORST_TIME;
        }
        set
        {
            PlayerTimeSaveValue_Legacy.CurrentValue = value.ToString();
            GameManager.GameInstance?.SaveData();
        }
    }

    public float BestTime => (float)BestTimeMS / 1000;

    public SaveValueDescriptor PlayerTimeSaveValue_Legacy;
}