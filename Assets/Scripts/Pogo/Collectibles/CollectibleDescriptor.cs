using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Collectibles
{
    [CreateAssetMenu(fileName = "c_", menuName = "Pogo/Collectibles/Descriptor")]
    public class CollectibleDescriptor : ScriptableObject
    {
        [System.Serializable]
        public enum UnlockTypes
        {
            SlotOnly,
            AccountOnly,
            AccountAndSlot
        }
        public UnlockTypes UnlockType;

        [System.Serializable]
        public enum CollectibleTypes
        {
            None,
            Key
        }

        public CollectibleTypes CollectibleType;

        public string Key => name;
        public GameObject NotificationPrefab;
    }
}
