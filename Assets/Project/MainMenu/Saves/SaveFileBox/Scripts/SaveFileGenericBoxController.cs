using UnityEngine.Events;
using UnityEngine;

namespace Pogo.Saving
{
    public class SaveFileGenericBoxController : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<SaveSlotIds> OnDeleteTriggered;
        public SaveSlotIds SlotId;

        protected Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        #region Deletion

        public float DeleteMenuButtonCooldownSeconds;
        private float LastDeleteMenuTime;
        private enum DeleteStates
        {
            Idle,
            Confirmation
        }
        private DeleteStates DeleteState = DeleteStates.Idle;

        public void SoftDelete()
        {
            if (DeleteState == DeleteStates.Idle)
            {
                StartDeleteConfirmation();
            }
            else if (DeleteState == DeleteStates.Confirmation)
            {
                HardDelete();
            }
        }

        public void StartDeleteConfirmation()
        {
            if (!TryNavigateDeleteMenu()) return;

            DeleteState = DeleteStates.Confirmation;
            animator.SetTrigger("Delete_Initiate");
        }

        public void CancelDeleteConfirmation()
        {
            if (!TryNavigateDeleteMenu()) return;
            if (DeleteState != DeleteStates.Confirmation)
            {
                return;
            }

            DeleteState = DeleteStates.Idle;
            animator.SetTrigger("Delete_Cancel");
        }

        public void HardDelete()
        {
            if (!TryNavigateDeleteMenu()) return;

            OnDeleteTriggered.Invoke(SlotId);
        }

        private bool TryNavigateDeleteMenu()
        {
            if (Time.unscaledTime < LastDeleteMenuTime + DeleteMenuButtonCooldownSeconds)
            {
                return false;
            }

            LastDeleteMenuTime = Time.unscaledTime;
            return true;
        }

        #endregion
    }
}
