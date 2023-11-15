using Inputter;
using Pogo.Difficulties;
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
            if (InputManager.CheckKeyDown(KeyName.Reset))
            {
                ResetPlayer();
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
                if (lastPress + holdInterval < Time.unscaledTime)
                {
                    pressQueued = false;
                    resetRespawn();
                }
                else if (InputManager.CheckKeyUp(KeyName.Checkpoint))
                {
                    pressQueued = false;
                    setRespawn();
                }
            }

            if (InputManager.CheckKeyDown(KeyName.Checkpoint))
            {
                if (PogoGameManager.PogoInstance.CurrentDifficulty == Difficulty.Assist)
                {
                    pressQueued = true;
                    lastPress = Time.unscaledTime;
                }
            }

        }

        private void ResetPlayer()
        {
            PogoGameManager.PogoInstance.ResetPlayer();
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
