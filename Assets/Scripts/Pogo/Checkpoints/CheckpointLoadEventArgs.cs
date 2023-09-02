using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Checkpoints
{
    public class CheckpointLoadEventArgs
    {
        public readonly ChapterDescriptor LoadedChapter;
        public readonly CheckpointDescriptor LoadedCheckpoint;

        public CheckpointLoadEventArgs(ChapterDescriptor loadedChapter, CheckpointDescriptor loadedCheckpoint)
        {
            LoadedChapter = loadedChapter;
            LoadedCheckpoint = loadedCheckpoint;
        }
    }
}
