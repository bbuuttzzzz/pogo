using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu( fileName = "gsound_", menuName = "Pogo/GlobalSoundDescriptor")]
    public class GlobalSoundDescriptor : ScriptableObject
    {
        public GameObject Prefab;
    }
}
