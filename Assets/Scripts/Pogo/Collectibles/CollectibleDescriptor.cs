using Pogo.Cosmetics;
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
            Coin,
            Cosmetic
        }

        public CollectibleTypes CollectibleType;
        public bool ExistsInWorld = true;

        public bool IgnoreForCompletion;

        [HideInInspector]
        public CosmeticDescriptor CosmeticDescriptor;
        [HideInInspector]
        public GameObject NotificationPrefab;
        [HideInInspector]
        public bool SpawnGenericNotification;
        [HideInInspector]
        public string GenericNotificationTitle;
        [HideInInspector]
        public string GenericNotificationTitle_HalfUnlocked;
        [HideInInspector]
        public string GenericNotificationBody;
        [HideInInspector]
        public string GenericNotificationBody_HalfUnlocked;
        [HideInInspector]
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
