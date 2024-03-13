using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps
{
    [CreateAssetMenu(fileName = "EntityPrefabManifest", menuName = "Pogo/CustomMaps/EntityPrefabManifest")]
    public class EntityPrefabManifest : ScriptableObject
    {
        public EntityPrefabEntry[] Items;
    }
}
