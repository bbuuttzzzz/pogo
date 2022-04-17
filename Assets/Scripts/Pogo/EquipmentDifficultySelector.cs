using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo
{
    public class EquipmentDifficultySelector : MonoBehaviour
    {
        public EquipmentDifficulty[] EquipmentDifficulties;

        public EquipmentTypeDescriptor EquipmentTypeToCheck;

        public void Start()
        {
            var equipper = GetComponent<Equipper>();
            EquipmentSlot equipmentSlot = equipper != null ? equipper.FindSlot(EquipmentTypeToCheck) : null;
            if (equipmentSlot != null)
            {
                UpdateDifficulty(equipmentSlot);
            }
        }

        public void UpdateDifficulty(EquipmentSlot equipmentSlot)
        {
            UpdateDifficulty(equipmentSlot.Equipment);
        }

        public void UpdateDifficulty(EquipmentDescriptor equipment)
        {
            foreach(EquipmentDifficulty equipmentDifficulty in EquipmentDifficulties)
            {
                if (equipmentDifficulty.Equipment == equipment)
                {
                    SetDifficulty(equipmentDifficulty.Difficulty);
                    return;
                }
            }

            SetDifficulty(PogoGameManager.Difficulty.Normal);
        }

        public void SetDifficulty(PogoGameManager.Difficulty difficulty)
        {
            if (PogoGameManager.GameInstanceIsValid())
                PogoGameManager.PogoInstance.CurrentDifficulty = difficulty;
        }
    }
}
