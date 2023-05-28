using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Abilities
{
    [CreateAssetMenu(fileName = "abil_", menuName = "Pogo/Ability", order = 1)]
    public class AbilityDescriptor : ScriptableObject
    {
        public GameObject Prefab;
    }
}
