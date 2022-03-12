using Inputter;
using System;
using System.Collections;
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
            if (GameInstance != this) return;

            RespawnPoint = InitialRespawnPoint;
            levelManager = GetComponent<PogoLevelManager>();
            RegisterGameSetting(new GameSettingFloat(KEY_FIELD_OF_VIEW, 90));
            RegisterGameSetting(new GameSettingFloat(KEY_SENSITIVITY, 0.1f));
            RegisterGameSetting(new GameSettingFloat(KEY_INVERT, 0f));

            OnPlayerDeath.AddListener(() => NumberOfDeaths++);

#if UNITY_EDITOR
#else
            LoadControlScene(MainMenuControlScene);       
#endif
        }
        protected override void Update()
        {
            base.Update();
            if (InputManager.CheckKeyDown(KeyName.Reset))
            {
                KillPlayer();
            }
            if (InputManager.CheckKeyDown(KeyName.Pause) && !InControlScene)
            {
                Paused = !Paused;
            }
        }

#region Level Management

        PogoLevelManager levelManager;
        [HideInInspector]
        public LevelDescriptor InitialLevel;

        public void LoadLevel(LevelDescriptor newLevel, bool isFirstLoad = false)
        {
#if UNITY_EDITOR
            if (DontLoadScenesInEditor) return;
#endif
            bool loadingFromMenu = CurrentControlScene != null;
            levelManager.LoadLevelAsync(newLevel, (levelLoadingData) => StartCoroutine(onCheckLevelProgress(levelLoadingData, loadingFromMenu)));
        }

        IEnumerator onCheckLevelProgress(LevelLoadingData levelLoadingData, bool loadingFromMenu)
        {
            foreach(AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = false;
            }

            bool finished = false;
            while( !finished )
            {
                float progress = 0;
                finished = true;

                string txt = "";
                foreach(AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
                {
                    progress += Task.isDone ? 1 : Task.progress / 0.9f;
                    finished = finished && (Task.progress >= 0.9f || Task.isDone);
                    txt = txt + Task.progress + " ";
                }

                progress /= levelLoadingData.LoadingSceneTasks.Count;
                Debug.Log($"Progress: %{(progress * 100):N2} -- {txt}");

                yield return new WaitForSeconds(0.02f);
            }

            foreach (AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = true;
            }
            if (loadingFromMenu)
            {
                UnloadControlScene();
                ResetStats();
            }
        }
        #endregion

        #region Player
        private PlayerController player;
        public PlayerController Player => player;

        public static void RegisterPlayer(PlayerController player)
        {
            PogoInstance.player = player;
        }

        public static void KillPlayer(KillType killType = null)
        {
            PogoInstance?.player.Die(killType);
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

        public override void LoadControlScene(ControlSceneDescriptor newScene, Action<List<AsyncOperation>> callback = null)
        {
            if (levelManager != null)
            {
                levelManager.ResetLoadedLevel();
            }
            base.LoadControlScene(newScene, callback);
        }

        public Transform InitialRespawnPoint;
        [HideInInspector]
        public Transform RespawnPoint;
#endregion

        #region Settings
        public static string KEY_FIELD_OF_VIEW = "FieldOfView";
        public static string KEY_SENSITIVITY = "Sensitivity";
        public static string KEY_INVERT = "InvertY";
        #endregion

        #region Stats
        public int SecretsFoundCount;
        public int NumberOfDeaths;
        public float GameStartTime;

        public void ResetStats()
        {
            SecretsFoundCount = 0;
            NumberOfDeaths = 0;
            GameStartTime = Time.time;
        }
        #endregion
    }
}
