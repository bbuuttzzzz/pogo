using Inputter;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

namespace Pogo
{
    public class CustomRespawnController : MonoBehaviour
    {
        public UnityEvent OnSetRespawn;
        public UnityEvent OnResetRespawn;

        private void Awake()
        {
            lastSet = Time.time - Cooldown + 1; // give it a 1 second cooldown when placed
        }

        private void Update()
        {
            UpdateCustomRespawn();
        }

        bool pressQueued = false;
        float lastPress;
        float lastSet;
        const float holdInterval = 1;
        public float Cooldown;

        private void UpdateCustomRespawn()
        {
            if (pressQueued)
            {
                if (lastPress + holdInterval < Time.time)
                {
                    pressQueued = false;
                    resetRespawn();
                }
                else if (InputManager.CheckKeyUp(KeyName.Checkpoint))
                {
                    pressQueued = false;
                    if (lastSet + Cooldown < Time.time)
                    {
                        setRespawn();
                    }
                }
            }

            if (InputManager.CheckKeyDown(KeyName.Checkpoint))
            {
                pressQueued = true;
                lastPress = Time.time;
            }

        }

        private void setRespawn()
        {
            if (GameManager.GameInstanceIsValid())
            {
                if (PogoGameManager.PogoInstance.RegisterCustomRespawnPoint(transform.position, transform.rotation.YawOnly()))
                {
                    lastSet = Time.time;
                    OnSetRespawn?.Invoke();
                }
            }
        }

        private void resetRespawn()
        {
            if (GameManager.GameInstanceIsValid())
            {
                if (PogoGameManager.PogoInstance.ResetCustomRespawnPoint())
                {
                    OnResetRespawn?.Invoke();
                }
            }
        }
    }
}
