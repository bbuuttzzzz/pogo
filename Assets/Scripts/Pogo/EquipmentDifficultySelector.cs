using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo
{
    public class EquipmentDifficultySelector : MonoBehaviour
    {
        public EquipmentTypeDescriptor EquipmentTypeToCheck;

        public EquipmentDescriptor[] HardModeEquipment;
        public EquipmentDescriptor[] FreeplayModeEquipment;
        public EquipmentDescriptor[] ExpertModeEquipment;

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
            foreach(EquipmentDescriptor descriptor in HardModeEquipment)
            {
                if (descriptor == equipment)
                {
                    SetDifficulty(PogoGameManager.Difficulty.Hard);
                    return;
                }
            }
            foreach (EquipmentDescriptor descriptor in FreeplayModeEquipment)
            {
                if (descriptor == equipment)
                {
                    SetDifficulty(PogoGameManager.Difficulty.Freeplay);
                    return;
                }
            }
            foreach (EquipmentDescriptor descriptor in ExpertModeEquipment)
            {
                if (descriptor == equipment)
                {
                    SetDifficulty(PogoGameManager.Difficulty.Expert);
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
