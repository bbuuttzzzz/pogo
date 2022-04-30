using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo
{
    public class DifficultyChangedEventArgs
    {
        public PogoGameManager.Difficulty InitialDifficulty;
        public PogoGameManager.Difficulty FinalDifficulty;

        public DifficultyChangedEventArgs(PogoGameManager.Difficulty initialDifficulty, PogoGameManager.Difficulty finalDifficulty)
        {
            InitialDifficulty = initialDifficulty;
            FinalDifficulty = finalDifficulty;
        }
    }
}
