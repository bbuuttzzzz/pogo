using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;

namespace Pogo.CustomMaps
{
    public abstract class FuncUnlockable : CollisionOrbTrigger
    {
        public ParticleSystemData[] ActivatedParticleSystems;
        public UnityEvent OnUnlocked;
        public UnityEvent OnLocked;

        protected PogoGameManager gameManager;
        new protected MeshCollider collider;
        new protected MeshRenderer renderer;

        public bool Invisible;
        public Material DefaultMaterial;

        public abstract bool CanUnlock();

        protected virtual void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;
            collider = GetComponent<MeshCollider>();
            renderer = GetComponent<MeshRenderer>();
            OnActivated.AddListener(Base_OnActivated);
        }

        public virtual void Respawn()
        {
            collider.enabled = true;
            renderer.enabled = true;
        }

        private void Base_OnActivated(CollisionEventArgs arg0)
        {
            if (CanUnlock())
            {
                Trigger();
            }
            else
            {
                gameManager.ShowTextCrawl(GetFailMessage(), 1);
                OnLocked?.Invoke();
            }
        }

        protected abstract string GetFailMessage();

        public void Trigger()
        {
            OnUnlocked?.Invoke();
            collider.enabled = false;
            renderer.enabled = false;
            foreach (var system in ActivatedParticleSystems)
            {
                system.System.Play();
            }
        }

        public void UpdateMesh()
        {
            var currentMesh = GetComponent<MeshFilter>().sharedMesh;

            foreach (var system in ActivatedParticleSystems)
            {
                var shape = system.System.shape;
                shape.mesh = currentMesh;
            }
        }

        [System.Serializable]
        public struct ParticleSystemData
        {
            public ParticleSystem System;
        }
    }
}
