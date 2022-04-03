using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

namespace Pogo
{
    public class EquipmentSelectorController : MonoBehaviour
    {
        public EquipmentUnlockDescriptor[] Equipment;
        [HideInInspector]
        public EquipmentUnlockDescriptor[] UnlockedEquipment;
        private int ActiveUnlockIndex = 0;

        public UnityEvent OnActiveUnlockChanged;
        public UnityStringEvent OnUnlockNameChanged;
        public UnityStringEvent OnUnlockDescriptionChanged;

        private void Start()
        {
            SetupUnlockedEquipment();

            OnActiveUnlockChanged.AddListener(onChanged);
            OnActiveUnlockChanged.AddListener(updateButtonInteractableStates);
            OnActiveUnlockChanged?.Invoke();

            if (IncrementButton != null) IncrementButton.onClick.AddListener(Increment);
            if (DecrementButton != null) DecrementButton.onClick.AddListener(Decrement);
        }

        private void SetupUnlockedEquipment()
        {
            List<EquipmentUnlockDescriptor> unlockedEquipmentList = new List<EquipmentUnlockDescriptor>();
            foreach(EquipmentUnlockDescriptor item in Equipment)
            {
                if (item.IsUnlocked)
                {
                    unlockedEquipmentList.Add(item);
                }
            }

            UnlockedEquipment = unlockedEquipmentList.ToArray();
        }

        void onChanged()
        {
            updateButtonInteractableStates();
            OnUnlockNameChanged.Invoke(UnlockedEquipment[ActiveUnlockIndex].Equipment.DisplayName);
            OnUnlockDescriptionChanged.Invoke(UnlockedEquipment[ActiveUnlockIndex].Equipment.Description);
            if(PogoGameManager.GameInstanceIsValid())
            {
                PogoGameManager.PogoInstance.Equip(UnlockedEquipment[ActiveUnlockIndex].Equipment);
            }
        }

        public Button IncrementButton; 
        public void Increment()
        {
            ActiveUnlockIndex = Math.Min(UnlockedEquipment.Length - 1, ActiveUnlockIndex + 1);
            OnActiveUnlockChanged?.Invoke();
        }

        public Button DecrementButton;
        public void Decrement()
        {
            ActiveUnlockIndex = Math.Max(0, ActiveUnlockIndex - 1);
            OnActiveUnlockChanged?.Invoke();
        }

        private void updateButtonInteractableStates()
        {
            DecrementButton.interactable = (ActiveUnlockIndex > 0);
            IncrementButton.interactable = (ActiveUnlockIndex < UnlockedEquipment.Length - 1);
        }
    }
}
