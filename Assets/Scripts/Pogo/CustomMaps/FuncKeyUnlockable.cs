using Pogo.CustomMaps.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps
{
    public class FuncKeyUnlockable : FuncUnlockable
    {
        public bool Collected;
        public UnlockMaterial[] Materials;
        public PickupIds PickupId;


        protected override void Awake()
        {
            base.Awake();
            gameManager = PogoGameManager.PogoInstance;

            gameManager.OnPickupCollected.AddListener(Parent_OnPickupCollected);
        }

        public void LoadDefaultMaterial()
        {
            foreach(var pickupMaterial in Materials)
            {
                if (pickupMaterial.Id == PickupId)
                {
                    GetComponent<MeshRenderer>().sharedMaterial = pickupMaterial.Material;
                    return;
                }
            }
            throw new KeyNotFoundException(PickupId.ToString());
        }
        private void Parent_OnPickupCollected(PickupCollectedEventArgs arg0)
        {
            if (arg0.PickupId == PickupId)
            {
                Collected = true;
                CheckAutoUnlock();
            }
        }

        public override bool CanUnlock() => Collected;

        public override void Respawn()
        {
            base.Respawn();
            Collected = false;
        }

        protected override string GetFailMessage()
        {
            string keyName = PickupId switch
            {
                PickupIds.RedKey => "Red Key",
                PickupIds.BlueKey => "Blue Key",
                PickupIds.YellowKey => "Yellow Key",
                _ => throw new KeyNotFoundException(PickupId.ToString()),
            };

            return $"Need {keyName}";
        }

        [System.Serializable]
        public struct UnlockMaterial
        {
            public PickupIds Id;
            public Material Material;
        }
    }
}
