using Pogo.Checkpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    public class GeneratedCheckpoint : Checkpoint
    {
        public override ChapterDescriptor Chapter => null;

        public CheckpointId Id;
        public override CheckpointId CheckpointId => Id;
        
        public override bool CanSkip { get; set; }
    }
}
