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
        private PogoGameManager gameManager;
        public int CoinsToUnlock;
        public int CoinsCollected;
        public int CoinsRemaining => math.max(CoinsToUnlock - CoinsCollected, 0);

        private Material localMaterial;

        protected override void Awake()
        {
            base.Awake();
            gameManager = PogoGameManager.PogoInstance;

            gameManager.OnPickupCollected.AddListener(Parent_OnPickupCollected);
        }

        private void OnDestroy()
        {
            gameManager.OnPickupCollected.RemoveListener(Parent_OnPickupCollected);
        }

        private void Parent_OnPickupCollected(PickupCollectedEventArgs arg0)
        {
            CoinsCollected++;
            UpdateShader();
        }

        public override void Respawn()
        {
            base.Respawn();
            CoinsCollected = 0;
            UpdateShader();
        }

        [ContextMenu("Update Shader Now")]
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

            localMaterial.SetFloat("_NumberValue", CoinsRemaining);
        }

        public override bool CanUnlock()
        {
            return CoinsRemaining == 0;
        }
    }
}
