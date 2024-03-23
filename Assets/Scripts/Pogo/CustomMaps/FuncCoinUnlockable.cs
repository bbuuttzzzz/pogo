using Assets.Scripts.Pogo.CustomMaps;
using Pogo.CustomMaps.Pickups;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;

namespace Pogo.CustomMaps
{
    public class FuncCoinUnlockable : FuncUnlockable
    {
        public int CoinsToUnlock;
        public int CoinsCollected;
        public Material DefaultMaterial;
        public int CoinsRemaining => math.max(CoinsToUnlock - CoinsCollected, 0);

        private Material localMaterial;

        protected override void Awake()
        {
            base.Awake();
            gameManager.OnPickupCollected.AddListener(Parent_OnPickupCollected);
        }

        private void OnDestroy()
        {
            gameManager.OnPickupCollected.RemoveListener(Parent_OnPickupCollected);
        }

        private void Parent_OnPickupCollected(PickupCollectedEventArgs arg0)
        {
            if (arg0.PickupId == PickupIds.Penny)
            {
                CoinsCollected++;
            }
        }

        public override void Respawn()
        {
            base.Respawn();
            CoinsCollected = 0;
        }

        public void UpdateShader()
        {
#if DEBUG
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("You should only do this ingame");
            }
#endif

            if (localMaterial == null)
            {
                localMaterial = Instantiate(renderer.sharedMaterial);
                localMaterial.name = $"Local Material for {nameof(FuncCoinUnlockable)} {name}";
                renderer.sharedMaterial = localMaterial;
            }

            localMaterial.SetFloat("_NumberValue", CoinsToUnlock);
        }

        public override bool CanUnlock()
        {
            return CoinsRemaining == 0;
        }

        protected override string GetFailMessage()
        {
            if (CoinsRemaining == 1) return "Need 1 more coin";
            return $"Need {CoinsRemaining} more coins";
        }
    }
}
