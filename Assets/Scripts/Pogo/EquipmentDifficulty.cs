using Pogo.Difficulties;
using System;
using WizardUtils.Equipment;

namespace Pogo
{
    [Serializable]
    public class EquipmentDifficulty
    {
        public EquipmentDescriptor Equipment;
        public Difficulty Difficulty;
    }
}
