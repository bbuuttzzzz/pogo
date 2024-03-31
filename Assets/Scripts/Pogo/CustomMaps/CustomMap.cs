using Assets.Scripts.Pogo.CustomMaps;
using BSPImporter;
using Pogo.Checkpoints;
using Pogo.CustomMaps.Errors;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.CustomMaps
{
    public class CustomMap
    {
        public MapHeader Header;
        public Dictionary<CheckpointId, GeneratedCheckpoint> Checkpoints;
        private int LastSidePathNumber;

        public GameObject PlayerStart;
        public GeneratedCheckpoint FirstCheckpoint;
        public Transform ForceInitialSpawn { get; private set; }
        private string ForceInitialSpawnName;
        public bool HasFinish;
        public GameObject InfoCameraThumbnailObject;
        public UnityEvent OnRestart;
        public Surfaces.SurfaceSource SurfaceSource { get; private set; }
        private List<MapError> errors;

        public int CollectedPennies = 0;
        public List<PickupIds> CollectedKeys;

        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public IEnumerable<MapError> Errors => errors;

        public CustomMap()
        {
            Checkpoints = new Dictionary<CheckpointId, GeneratedCheckpoint>();
            SurfaceSource = new Surfaces.SurfaceSource();
            errors = new List<MapError>();
            OnRestart = new UnityEvent();
            CollectedKeys = new List<PickupIds>();
            LastSidePathNumber = -1;
        }

        public void ResetAll()
        {
            CollectedPennies = 0;
            CollectedKeys = new List<PickupIds>();
            OnRestart.Invoke();
        }

        public void AddPickup(PickupIds pickup)
        {
            if (pickup == PickupIds.Penny)
            {
                CollectedPennies++;
            }
            else
            {
                CollectedKeys.Add(pickup);
            }
        }

        public void AddError(MapError error)
        {
            if (error.Severity == MapError.Severities.Error)
            {
                ErrorCount++;
            }
            else if (error.Severity == MapError.Severities.Warning)
            {
                WarningCount++;
            }

            errors.Add(error);
        }

        public bool HasError => ErrorCount > 0;

        public void RegisterCheckpoint(GeneratedCheckpoint checkpoint)
        {
            if (checkpoint.CheckpointId.CheckpointType == CheckpointTypes.SidePath)
            {
                checkpoint.Id = new CheckpointId(CheckpointTypes.SidePath, ++LastSidePathNumber);
            }

            Checkpoints.Add(checkpoint.Id, checkpoint);

            if (checkpoint.CheckpointId.CheckpointType == CheckpointTypes.MainPath
                && (FirstCheckpoint == null || checkpoint.Id.CheckpointNumber < FirstCheckpoint.CheckpointId.CheckpointNumber))
            {
                FirstCheckpoint = checkpoint;
            }
        }

        public void RegisterForceInitialSpawn(Transform spawnPoint, string targetName)
        {
            if (ForceInitialSpawn != null)
            {
                AddError(new MapError(null, $"Multiple info_player_respawn are set with Force Initial Spawn = 1! '{targetName}' and '{ForceInitialSpawnName}'", MapError.Severities.Error));
                return;
            }

            ForceInitialSpawn = spawnPoint;
            ForceInitialSpawnName = targetName;
        }
    }
}
