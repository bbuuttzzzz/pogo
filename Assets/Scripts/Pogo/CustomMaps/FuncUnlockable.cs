using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WizardPhysics;
using WizardUtils.Extensions;

namespace Pogo.CustomMaps
{
    public abstract class FuncUnlockable : CollisionOrbTrigger
    {
        public Transform[] ActivatedAudioSourceTransforms;
        public ParticleSystemData[] ActivatedParticleSystems;
        public UnityEvent OnUnlocked;
        public UnityEvent OnLocked;
        public float TextCrawlDelaySeconds = 0.75f;
        public bool AutoUnlock;

        protected PogoGameManager gameManager;
        new protected MeshCollider collider;
        new protected MeshRenderer renderer;

        public bool Invisible;

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

        protected void CheckAutoUnlock()
        {
            if (AutoUnlock && CanUnlock())
            {
                Trigger();
            }
        }

        private void Base_OnActivated(CollisionEventArgs arg0)
        {
            if (CanUnlock())
            {
                Trigger();
            }
            else
            {
                gameManager.ShowTextCrawl(GetFailMessage(), TextCrawlDelaySeconds);
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
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            var currentMesh = meshFilter.sharedMesh;

            foreach (var system in ActivatedParticleSystems)
            {
                var shape = system.System.shape;
                shape.mesh = currentMesh;
            }

            foreach(var audioTransform in ActivatedAudioSourceTransforms)
            {
                audioTransform.position = currentMesh.bounds.center;
            }
        }

        [System.Serializable]
        public struct ParticleSystemData
        {
            public ParticleSystem System;
        }
    }
}
