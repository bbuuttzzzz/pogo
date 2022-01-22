using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardEffects
{
    using UnityEngine;

    [System.Serializable]
    public struct EffectData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 origin;
        public Transform parent;
    }
}
