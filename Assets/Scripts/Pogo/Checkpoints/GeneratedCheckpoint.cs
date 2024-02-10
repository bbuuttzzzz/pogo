using Assets.Scripts.Pogo.Checkpoints;
using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo
{
    public class GeneratedCheckpoint : Checkpoint
    {
        public bool Invisible;
        public ParticleSystem ActivatedParticleSystem;

        public override ChapterDescriptor Chapter => null;

        public CheckpointId Id;
        public override CheckpointId CheckpointId => Id;
        
        public override bool CanSkip { get; set; }

        private void Awake()
        {
            PogoGameManager.PogoInstance.OnRespawnPointChanged.AddListener(GameManager_OnRespawnPointChanged);
        }

        private void OnDestroy()
        {
            PogoGameManager.PogoInstance.OnRespawnPointChanged.RemoveListener(GameManager_OnRespawnPointChanged);
        }

        public void UpdateMesh()
        {
            var currentMesh = GetComponent<MeshFilter>().sharedMesh;
            var shape = ActivatedParticleSystem.shape;
            shape.mesh = currentMesh;
        }

        private void GameManager_OnRespawnPointChanged(RespawnPointChangedEventArgs arg0)
        {
            bool wasLastCheckpoint = arg0.OldSpawnPoint.transform == RespawnPoint;
            bool isNewCheckpoint = arg0.NewSpawnPoint.transform == RespawnPoint;

            if (wasLastCheckpoint && !isNewCheckpoint)
            {
                // lose active checkpoint, set renderer enabled if not invisible
                GetComponent<Renderer>().enabled = !Invisible;
            }
            else if (!wasLastCheckpoint && isNewCheckpoint)
            {
                // become active checkpoint, set renderer invisible
                GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
