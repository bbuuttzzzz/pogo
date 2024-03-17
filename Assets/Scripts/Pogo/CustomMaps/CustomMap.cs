using Assets.Scripts.Pogo.CustomMaps;
using BSPImporter;
using Pogo.Checkpoints;
using Pogo.CustomMaps.Errors;
using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps
{
    public class CustomMap
    {
        public MapHeader Header;
        public Dictionary<CheckpointId, GeneratedCheckpoint> Checkpoints;
        public GameObject PlayerStart;
        public GeneratedCheckpoint FirstCheckpoint;
        public bool HasFinish;
        public GameObject InfoCameraThumbnailObject;
        public Surfaces.SurfaceSource SurfaceSource { get; private set; }
        private List<MapError> errors;
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public IEnumerable<MapError> Errors => errors;

        public CustomMap()
        {
            Checkpoints = new Dictionary<CheckpointId, GeneratedCheckpoint>();
            SurfaceSource = new Surfaces.SurfaceSource();
            errors = new List<MapError>();
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
            Checkpoints.Add(checkpoint.Id, checkpoint);

            if (checkpoint.CheckpointId.CheckpointType == CheckpointTypes.MainPath
                && (FirstCheckpoint == null || checkpoint.Id.CheckpointNumber < FirstCheckpoint.CheckpointId.CheckpointNumber))
            {
                FirstCheckpoint = checkpoint;
            }
        }
    }
}
