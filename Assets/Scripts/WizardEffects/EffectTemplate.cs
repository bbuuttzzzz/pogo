using System.Collections;
using UnityEngine;

namespace WizardEffects
{
    [CreateAssetMenu(fileName = "new Effect Template", menuName = "ScriptableObjects/WizardEffects/EffectTemplate", order = 1)]
    public class EffectTemplate : ScriptableObject
    {
        public GameObject Prefab;
        public bool RegisterEffect;
        [Tooltip("for things that should linger in the world as long as possible after the effect finishes (like bullet holes)")]
        public bool IsLastingEffect;
    }
}
