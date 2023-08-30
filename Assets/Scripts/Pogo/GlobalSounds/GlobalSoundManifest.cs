using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu(fileName = "GlobalSoundManifest", menuName = "Pogo/GlobalSoundManifest")]
    public class GlobalSoundManifest : ScriptableObject
    {
        public GlobalSoundDescriptor[] Descriptors;
    }
}
