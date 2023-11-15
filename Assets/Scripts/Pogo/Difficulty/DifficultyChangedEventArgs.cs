using Pogo.Difficulties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    public class DifficultyChangedEventArgs
    {
        public Difficulty InitialDifficulty;
        public Difficulty FinalDifficulty;

        public DifficultyChangedEventArgs(Difficulty initialDifficulty, Difficulty finalDifficulty)
        {
            InitialDifficulty = initialDifficulty;
            FinalDifficulty = finalDifficulty;
        }
    }
}
