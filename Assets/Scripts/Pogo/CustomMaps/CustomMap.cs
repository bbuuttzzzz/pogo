using BSPImporter;
using Pogo.Checkpoints;
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
        public TriggerFinish Finish;
        public GameObject InfoCameraThumbnailObject;
        public Surfaces.SurfaceSource SurfaceSource { get; private set; }

        public CustomMap()
        {
            Checkpoints = new Dictionary<CheckpointId, GeneratedCheckpoint>();
            SurfaceSource = new Surfaces.SurfaceSource();
        }

        public void RegisterCheckpoint(GeneratedCheckpoint checkpoint)
        {
            Checkpoints.Add(checkpoint.Id, checkpoint);

            if (checkpoint.CheckpointId.CheckpointType == CheckpointTypes.MainPath
                && (FirstCheckpoint == null || checkpoint.Id.CheckpointNumber < FirstCheckpoint.CheckpointId.CheckpointNumber))
            {
                FirstCheckpoint = checkpoint;
            }
        }

        public void RegisterFinish(TriggerFinish finish)
        {
            if (Finish != null)
            {
                throw new FormatException("Map contains multiple Trigger_Finish! it should have at MOST 1!!!");
            }

            Finish = finish;
        }
    }
}
