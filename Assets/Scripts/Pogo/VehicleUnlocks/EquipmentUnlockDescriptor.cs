using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.Equipment;
using WizardUtils.Saving;

namespace Pogo
{
    [CreateAssetMenu(fileName = "EquipmentUnlockDescriptor", menuName = "Pogo/EquipmentUnlockDescriptor", order = 1)]
    public class EquipmentUnlockDescriptor : ScriptableObject
    {
        public EquipmentDescriptor Equipment;

        #region Unlocking
        public SaveValueDescriptor UnlockedSaveValue;
        public bool IsUnlocked
        {
            get
            {
                return UnlockedSaveValue == null
                    || GameManager.GameInstance?.GetMainSaveValue(UnlockedSaveValue) == "1";
            }
            set
            {
                GameManager.GameInstance?.SetMainSaveValue(UnlockedSaveValue, value ? "1" : "0");
            }
        }

        #endregion
    }
}