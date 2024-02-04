using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    public interface ICheckpoint
    {
        public ChapterDescriptor Chapter { get; }
        public CheckpointId CheckpointId { get; }

        public void NotifyCheckpointLoad(ICheckpoint other);
    }
}
