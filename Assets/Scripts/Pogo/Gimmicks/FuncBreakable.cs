using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;

namespace Pogo.Gimmicks
{
    public class FuncBreakable : CollisionOrbTrigger
    {
        public bool RegenerateOnPlayerSpawn;

        public AudioSource AudioSource;
        public ParticleSystemData[] ActivatedParticleSystems;
        new private MeshCollider collider;
        new private MeshRenderer renderer;

        private void Awake()
        {
            collider = GetComponent<MeshCollider>();
            renderer = GetComponent<MeshRenderer>();
            OnActivated.AddListener(Base_OnActivated);
            PogoGameManager.PogoInstance.OnPlayerSpawn.AddListener(GameManager_OnPlayerSpawn);
        }

        private void GameManager_OnPlayerSpawn()
        {
            if (RegenerateOnPlayerSpawn)
            {
                Regenerate();
            }
        }

        private void Base_OnActivated(CollisionEventArgs arg0)
        {
            Trigger();
        }

        public void Trigger()
        {
            collider.enabled = false;
            renderer.enabled = false;
            AudioSource.Play();
            foreach (var system in ActivatedParticleSystems)
            {
                system.System.Play();
            }
        }

        public void Regenerate()
        {
            collider.enabled = true;
            renderer.enabled = true;
        }

        public void UpdateMesh()
        {
            var currentMesh = GetComponent<MeshFilter>().sharedMesh;
            Material currentMaterial = GetComponent<MeshRenderer>().sharedMaterial;

            foreach (var system in ActivatedParticleSystems)
            {
                var shape = system.System.shape;
                shape.mesh = currentMesh;
                if (system.SetMaterial)
                {
                    var renderer = system.System.GetComponent<ParticleSystemRenderer>();
                    renderer.material = currentMaterial;
                }
            }
        }

        [System.Serializable]
        public struct ParticleSystemData
        {
            public ParticleSystem System;
            public bool SetMaterial;
        }
    }

}