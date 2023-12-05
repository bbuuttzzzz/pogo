using Pogo.Collectibles;
using System.Collections;
using System.Linq;
using UnityEngine;
using WizardUtils;
using WizardUtils.ManifestPattern;
using WizardUtils.Saving;

[CreateAssetMenu(fileName = "ChallengePack", menuName = "Pogo/ChallengePack", order = 1)]
public class ChallengePackDescriptor : ScriptableObject, IDescriptorManifest<DeveloperChallenge>
{
    public string PrintName;
    public DeveloperChallenge[] Challenges;

    #region Unlocking
    public CollectibleDescriptor Collectible;

    public bool IsUnlocked => Collectible == null || Collectible.CollectedInGlobalSave;

    public void Add(DeveloperChallenge descriptor)
    {
        ArrayHelper.InsertAndResize(ref Challenges, descriptor);
    }

    public bool Contains(DeveloperChallenge descriptor)
    {
        return Challenges.Contains(descriptor);
    }

    public void Remove(DeveloperChallenge descriptor)
    {
        ArrayHelper.DeleteAndResize(ref Challenges, descriptor);
    }

    #endregion
}