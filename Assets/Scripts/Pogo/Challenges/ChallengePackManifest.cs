using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.ManifestPattern;

namespace Pogo
{
    [CreateAssetMenu(fileName = "ChallengePackManifest", menuName = "Pogo/Challenges/PackManifest", order = 1)]
    public class ChallengePackManifest : ScriptableObject, IDescriptorManifest<ChallengePackDescriptor>
    {
        public ChallengePackDescriptor[] Items;

        private void OnEnable()
        {
            Items ??= new ChallengePackDescriptor[0];
        }

        public void Add(ChallengePackDescriptor descriptor)
        {
            ArrayHelper.InsertAndResize(ref Items, descriptor);
        }

        public bool Contains(ChallengePackDescriptor descriptor)
        {
            return Items.Contains(descriptor);
        }

        public void Remove(ChallengePackDescriptor descriptor)
        {
            ArrayHelper.DeleteAndResize(ref Items, descriptor);
        }
    }
}
