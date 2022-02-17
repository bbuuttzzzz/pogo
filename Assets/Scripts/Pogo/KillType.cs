using System.Collections;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu(fileName = "KillType", menuName = "ScriptableObjects/KillType", order = 1)]
    public class KillType : ScriptableObject
    {
        public AudioClip[] Sounds;

        public AudioClip RandomSound => Sounds == null ? null
            : Sounds[Random.Range(0, Sounds.Length)];
    }
}