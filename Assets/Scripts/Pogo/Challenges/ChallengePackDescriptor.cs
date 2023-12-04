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

    #endregion
}