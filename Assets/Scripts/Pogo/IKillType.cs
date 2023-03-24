using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public interface IKillType
    {
        public AudioClip[] Sounds { get; }
        public string EffectName { get; }

        public AudioClip RandomSound => Sounds == null ? null
            : Sounds[UnityEngine.Random.Range(0, Sounds.Length)];
    }
}
