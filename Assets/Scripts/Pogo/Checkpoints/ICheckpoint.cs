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
    public interface ICheckpoint
    {
        public ChapterDescriptor Chapter { get; }
        public CheckpointId Id { get; }
        public Transform SpawnPoint { get; }
        public bool CanSkip { get; set; }
        public UnityEvent OnSkip { get; }

        public string ToString()
        {
            return $"Checkpoint {Id} ({GetType()})";
        }
    }
}
