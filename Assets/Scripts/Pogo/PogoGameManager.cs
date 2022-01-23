using Inputter;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

namespace Pogo
{
    public class PogoGameManager : WizardUtils.GameManager
    {
        public static PogoGameManager PogoInstance => GameInstance as PogoGameManager;


        protected override void Awake()
        {
            base.Awake();

            RegisterGameSetting(new GameSettingFloat(KEY_FIELD_OF_VIEW, 90));
            RegisterGameSetting(new GameSettingFloat(KEY_SENSITIVITY, 0.1f));
            RegisterGameSetting(new GameSettingFloat(KEY_INVERT, 1f));
        }
        protected override void Update()
        {
            base.Update();
            if (InputManager.CheckKeyDown(KeyName.Reset))
            {
                KillPlayer();
            }
            if (InputManager.CheckKeyDown(KeyName.Pause))
            {
                Paused = !Paused;
            }
        }

        #region Player
        private PlayerController player;
        public PlayerController Player => player;

        public static void RegisterPlayer(PlayerController player)
        {
            PogoInstance.player = player;
        }

        public static void KillPlayer()
        {
            PogoInstance?.player.Die();
        }


        public UnityEvent OnPlayerDeath;
        #endregion

        #region Respawn Point
        public static bool TryRegisterRespawnPoint(Transform newRespawnPoint)
        {
            if (PogoInstance == null)
            {
                Debug.LogWarning("Tried to register a RespawnPoint with no CheckpointManager");
                return false;
            }

            if (newRespawnPoint == PogoInstance.RespawnPoint) return false;

            PogoInstance.RespawnPoint = newRespawnPoint;

            return true;

        }

        public Transform RespawnPoint;
        #endregion

        #region Settings
        public static string KEY_FIELD_OF_VIEW = "FieldOfView";
        public static string KEY_SENSITIVITY = "Sensitivity";
        public static string KEY_INVERT = "Invert";
        #endregion

    }
}
