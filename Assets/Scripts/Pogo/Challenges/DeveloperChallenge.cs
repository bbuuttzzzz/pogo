using Pogo.Challenges;
using System.Collections;
using UnityEngine;
using WizardUtils;
using WizardUtils.Saving;

[CreateAssetMenu(fileName = "DeveloperChallenge", menuName = "Pogo/DeveloperChallenge", order = 1)]
public class DeveloperChallenge : ScriptableObject
{
    public string DisplayName;
    public Challenge Challenge;

    public int BestTimeMS
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