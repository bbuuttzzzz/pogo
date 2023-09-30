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
            GlobalOnly,
            SlotAndGlobal
        }
        public UnlockTypes UnlockType => CollectibleType switch
        {
            CollectibleTypes.Key => UnlockTypes.SlotOnly,
            CollectibleTypes.ChallengePack => UnlockTypes.SlotAndGlobal,
            CollectibleTypes.Coin => UnlockTypes.SlotAndGlobal,
            _ => UnlockTypes.SlotAndGlobal
        };

        [System.Serializable]
        public enum CollectibleTypes
        {
            None,
            Key,
            ChallengePack,
            Coin
        }

        public CollectibleTypes CollectibleType;
        public GameObject NotificationPrefab;
        public bool SpawnGenericNotification;
        public string GenericNotificationTitle;
        public string GenericNotificationTitle_HalfUnlocked;
        public string GenericNotificationBody;
        public string GenericNotificationBody_HalfUnlocked;
        public GameObject GenericNotification3DIcon;

        #region Editor Stuff
#if UNITY_EDITOR
        [HideInInspector]
        public int SceneBuildIndex = -1;
        [HideInInspector, SerializeField]
        private GameObject self;
#endif
        #endregion


        public CollectibleStates GetState()
        {
            if (CollectedInSlotSave)
            {
                return CollectibleStates.Collected;
            }
            else if (CollectedInGlobalSave)
            {
                return CollectibleStates.HalfCollected;
            }
            else
            {
                return CollectibleStates.Uncollected;
            }
        }

        public bool CollectedInSlotSave
        {
            get
            {
                if (PogoGameManager.PogoInstance.CurrentSlotDataTracker == null)
                {
                    return false;
                }

                var data = PogoGameManager.PogoInstance.CurrentSlotDataTracker.GetCollectible(Key);
                return data.isUnlocked;
            }
        }

        public bool CollectedInGlobalSave
        {
            get
            {
                if (PogoGameManager.PogoInstance.CurrentGlobalDataTracker == null)
                {
                    return false;
                }

                var data = PogoGameManager.PogoInstance.CurrentGlobalDataTracker.GetCollectible(Key);
                return data.isUnlocked;
            }
        }

        public string Key => name;
    }
}
