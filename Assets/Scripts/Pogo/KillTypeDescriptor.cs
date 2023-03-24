using System.Collections;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu(fileName = "KillType", menuName = "ScriptableObjects/KillType", order = 1)]
    public class KillTypeDescriptor : ScriptableObject, IKillType
    {
        public AudioClip[] Sounds;
        public string EffectName;

        AudioClip[] IKillType.Sounds => Sounds;

        string IKillType.EffectName => EffectName;
    }
}