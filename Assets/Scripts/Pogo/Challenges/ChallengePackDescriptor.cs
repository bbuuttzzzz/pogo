using System.Collections;
using UnityEngine;
using WizardUtils;
using WizardUtils.Saving;

[CreateAssetMenu(fileName = "ChallengePack", menuName = "Pogo/ChallengePack", order = 1)]
public class ChallengePackDescriptor : ScriptableObject
{
    public string PrintName;
    public DeveloperChallenge[] Challenges;

    #region Unlocking
    public SaveValueDescriptor UnlockedSaveValue;
    public bool IsUnlocked
    {
        get
        {
            return UnlockedSaveValue == null
                || GameManager.GameInstance?.GetMainSaveValue(UnlockedSaveValue) == "1";
        }
        set
        {
            GameManager.GameInstance?.SetMainSaveValue(UnlockedSaveValue, value ? "1" : "0");
        }
    }

    #endregion
}