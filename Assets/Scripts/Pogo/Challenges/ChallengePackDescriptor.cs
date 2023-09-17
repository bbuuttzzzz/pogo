using Pogo.Collectibles;
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
    public CollectibleDescriptor Collectible;

    public bool IsUnlocked => Collectible == null || Collectible.CollectedInGlobalSave;

    public SaveValueDescriptor UnlockedSaveValue_Legacy;

    public bool IsUnlocked_Legacy
    {
        get
        {
            return UnlockedSaveValue_Legacy == null
                || GameManager.GameInstance?.GetMainSaveValue(UnlockedSaveValue_Legacy) == "1";
        }
        set
        {
            GameManager.GameInstance?.SetMainSaveValue(UnlockedSaveValue_Legacy, value ? "1" : "0");
        }
    }

    #endregion
}