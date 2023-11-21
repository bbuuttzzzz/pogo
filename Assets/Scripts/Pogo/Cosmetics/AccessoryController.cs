using Players.Visuals;
using System;
using UnityEngine;
using WizardUtils.Equipment;

namespace Pogo.Cosmetics
{
    public class AccessoryController : MonoBehaviour, IEquipmentRoot
    {
        public PlayerModelAttachment Attachment;

        IPlayerModelControllerProvider currentPlayerModel;

        public void OnEquipped()
        {
            AttachToParent();
        }

        void Start()
        {
            if (currentPlayerModel == null)
            {
                AttachToParent();
            }
        }

        public void OnUnequipped()
        {
            if (currentPlayerModel != null
                && Attachment != null)
            {
                currentPlayerModel.PlayerModelController.RemoveAttachment(Attachment);
            }
        }

        private void AttachToParent()
        {
            currentPlayerModel = GetComponentInParent<IPlayerModelControllerProvider>(true);
            if (currentPlayerModel == null)
            {
                Debug.LogWarning("missing PlayerController above AccessoryController", this);
                return;
            }

            if (Attachment != null)
            {
                currentPlayerModel.OnModelControllerChanged?.AddListener(Player_OnModelControllerChanged);
                if (currentPlayerModel.PlayerModelController != null)
                {
                    currentPlayerModel.PlayerModelController.AddAttachment(Attachment);
                }
            }
        }


        private void Player_OnModelControllerChanged(PlayerModelController arg0)
        {
            // ...LETS JUST PRAY THE OLD PLAYERMODEL CLEANED THIS UP FOR U :   ^   )
            arg0.AddAttachment(Attachment);
        }
    }
}
