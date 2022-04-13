using Inputter;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

namespace Pogo
{
    public class RespawnController : MonoBehaviour
    {
        public UnityEvent OnSetRespawn;
        public UnityEvent OnResetRespawn;

        private void Awake()
        {
            lastSet = Time.time - Cooldown + 1; // give it a 1 second cooldown when placed
        }

        private void Update()
        {
            if (InputManager.CheckKeyDown(KeyName.Checkpoint))
            {
                setRespawn();
            }
            CheckRespawnButton();
        }

        bool pressQueued = false;
        float lastPress;
        float lastSet;
        const float holdInterval = 1;
        public float Cooldown;

        private void CheckRespawnButton()
        {
            if (pressQueued)
            {
                if (lastPress + holdInterval < Time.time)
                {
                    pressQueued = false;
                    resetRespawn();
                }
                else if (InputManager.CheckKeyUp(KeyName.Reset))
                {
                    pressQueued = false;
                    killPlayer();
                }
            }

            if (InputManager.CheckKeyDown(KeyName.Reset))
            {
                if (PogoGameManager.PogoInstance.CurrentDifficulty == PogoGameManager.Difficulty.Freeplay)
                {
                    pressQueued = true;
                    lastPress = Time.time;
                }
                else
                {
                    killPlayer();
                }
            }

        }

        private void killPlayer()
        {
            PogoGameManager.KillPlayer();
        }

        private void setRespawn()
        {
            if (lastSet + Cooldown < Time.time
                && GameManager.GameInstanceIsValid()
                && PogoGameManager.PogoInstance.RegisterCustomRespawnPoint(transform.position, transform.rotation.YawOnly())
                )
            {
                lastSet = Time.time;
                OnSetRespawn?.Invoke();
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
