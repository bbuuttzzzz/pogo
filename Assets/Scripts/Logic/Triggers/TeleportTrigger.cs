using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Gimmicks
{
    public class TeleportTrigger : Trigger
    {
        private PogoGameManager gameManager;

        public ParticleSystem ActivatedParticleSystem;
        public Material DefaultMaterial;
        public Transform RespawnPoint;
        public bool PreservePhysics;
        public float PhysicsReorientAngle;
        public Vector3 InitialVelocity;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;
            base.OnActivated.AddListener(base_OnActivated);
        }

        private void base_OnActivated()
        {
            if (PreservePhysics)
            {
                gameManager.Player.TeleportToAndPreservePhysics(RespawnPoint, PhysicsReorientAngle);
            }
            else
            {
                gameManager.Player.TeleportTo(new TeleportData(transform, InitialVelocity));
            }
        }
        public void UpdateMesh()
        {
            var currentMesh = GetComponent<MeshFilter>().sharedMesh;
            var shape = ActivatedParticleSystem.shape;
            shape.mesh = currentMesh;
        }
    }
}
