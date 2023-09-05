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
        private List<CheckpointTrigger> checkpointTriggers = new List<CheckpointTrigger>();
        public ReadOnlyCollection<CheckpointTrigger> CheckpointTriggers => checkpointTriggers.AsReadOnly();

        public CheckpointManifest()
        {
            checkpointTriggers = new();
        }

        public void Add(CheckpointTrigger checkpointTrigger)
        {
            checkpointTriggers.Add(checkpointTrigger);
        }
    }
}
