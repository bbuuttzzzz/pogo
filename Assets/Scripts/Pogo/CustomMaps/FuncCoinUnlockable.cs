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

        protected override void Awake()
        {
            base.Awake();
            gameManager = GetComponent<PogoGameManager>();

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

        public void UpdateShader()
        {
            renderer.materials[0].SetFloat("NumberValue", CoinsRemaining);
        }

        public override bool CanUnlock()
        {
            return CoinsRemaining == 0;
        }
    }
}
