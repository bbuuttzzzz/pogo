using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Checkpoints
{
    public class CheckpointManifest
    {
        private List<ICheckpoint> checkpoints = new List<ICheckpoint>();
        public ReadOnlyCollection<ICheckpoint> Checkpoints => checkpoints.AsReadOnly();

        public CheckpointManifest()
        {
            checkpoints = new();
        }

        public void Add(ICheckpoint ICheckpoint)
        {
            checkpoints.Add(ICheckpoint);
        }

        public void Remove(ICheckpoint ICheckpoint)
        {
            checkpoints.Remove(ICheckpoint);
        }

        public void Set(ICollection<ICheckpoint> ICheckpoints)
        {
            this.checkpoints = new List<ICheckpoint>(ICheckpoints);
        }
    }
}
