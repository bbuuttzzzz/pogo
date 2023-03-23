using Inputter;
using Pogo.Challenges;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;
using WizardUtils.Equipment;
using WizardUtils.SceneManagement;

namespace Pogo
{
    public class PogoGameManager : WizardUtils.GameManager
    {
        public static PogoGameManager PogoInstance => GameInstance as PogoGameManager;


        protected override void Awake()
        {
            base.Awake();
            if (GameInstance != this) return;

            RespawnPoint = CachedRespawnPoint;
            levelManager = GetComponent<PogoLevelManager>();

#if UNITY_EDITOR
            if (InitialLevel != null)
            {
                levelManager.SetCurrentLevelInEditor(InitialLevel);
            }
#endif
            RegisterGameSetting(new GameSettingFloat(KEY_FIELD_OF_VIEW, 90));
            RegisterGameSetting(new GameSettingFloat(KEY_SENSITIVITY, 0.1f));
            RegisterGameSetting(new GameSettingFloat(KEY_INVERT, 0f));
            RegisterGameSetting(new GameSettingFloat(KEY_TIMER, 0f));

            OnPauseStateChanged += ((_, _) => UpdateTimeFreeze());
            OnPlayerDeath.AddListener(() => NumberOfDeaths++);
            OnPlayerDeath.AddListener(() => ResetLoadedLevel());
            OnSoftQuit += onSoftQuit;
            CustomCheckpoint.OnPlaced.AddListener(() => OnCustomCheckpointChanged?.Invoke(this, EventArgs.Empty));
#if UNITY_EDITOR
#else
            LoadControlScene(MainMenuControlScene);       
#endif
        }

        protected override void Update()
        {
            base.Update();
            if (InputManager.CheckKeyDown(KeyName.Pause) && !InControlScene)
            {
                Paused = !Paused;
            }
        }

        private void onSoftQuit(object sender, EventArgs e)
        {
            ResetCustomRespawnPoint(true);
        }


        #region Level Management
        public PogoLevelManager LevelManager => levelManager;
        PogoLevelManager levelManager;
        public LevelDescriptor InitialLevel;

        public UnityEvent OnLevelLoaded;

        bool isLoadingLevel;
        LevelDescriptor queuedLevel;

#if UNITY_EDITOR
        public override void LoadControlSceneInEditor(ControlSceneDescriptor newScene)
        {
            base.LoadControlSceneInEditor(newScene);
            var levelManager = GetComponent<PogoLevelManager>();

            levelManager.LoadDefaultAtmosphere();
        }
#endif

        public void LoadLevel(LevelDescriptor newLevel)
        {
            LoadLevel(newLevel, LevelLoadingSettings.Default);
        }

        public void LoadLevel(LevelDescriptor newLevel, LevelLoadingSettings settings)
        {
#if UNITY_EDITOR
            if (DontLoadScenesInEditor) return;
#endif
            if (isLoadingLevel)
            {
                queuedLevel = newLevel;
                return;
            }

            UnityAction call = null;
            call = () => OnLoadLevelFinished(call, settings);
            OnLevelLoaded.AddListener(call);
            isLoadingLevel = true;

            settings.LoadingFromMenu = settings.LoadingFromMenu || CurrentControlScene != null;
#if UNITY_WEBGL
            if (!levelManager.LoadLevelAsync(newLevel, settings, (levelLoadingData) => StartCoroutine(loadLevelsInOrder(levelLoadingData, settings))))
#else
            if (!levelManager.LoadLevelAsync(newLevel, settings, (levelLoadingData) => StartCoroutine(loadLevelsSimultaneous(levelLoadingData, settings))))
#endif
            {
                isLoadingLevel = false;
            }
        }

        void OnLoadLevelFinished(UnityAction call, LevelLoadingSettings settings)
        {
            LevelManager.TransitionAtmosphere(LevelManager.CurrentLevel, settings.InstantChangeAtmosphere);
            OnLevelLoaded.RemoveListener(call);
        }

        IEnumerator loadLevelsSimultaneous(LevelLoadingData levelLoadingData, LevelLoadingSettings settings)
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

                yield return new WaitForSecondsRealtime(0.02f);
            }

            foreach (AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = true;
            }
            finished = false;
            while (!finished )
            {
                float progress = 0;
                finished = true;

                string txt = "";
                foreach (AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
                {
                    progress += Task.isDone ? 1 : Task.progress;
                    finished = finished && Task.isDone;
                    txt = txt + Task.progress + " ";
                }

                progress /= levelLoadingData.LoadingSceneTasks.Count;
                Debug.Log($"Progress: %{(progress * 100):N2} -- {txt}");

                yield return new WaitForSecondsRealtime(0.02f);
            }

            if (settings.LoadingFromMenu)
            {
                UnloadControlScene();
                ResetStats();
            }

            isLoadingLevel = false;
            OnLevelLoaded?.Invoke();
        }

        IEnumerator loadLevelsInOrder(LevelLoadingData levelLoadingData, LevelLoadingSettings settings)
        {
            foreach (AsyncOperation task in levelLoadingData.LoadingSceneTasks)
            {
                task.allowSceneActivation = false;
            }

            int completed = 0;
            foreach(AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
            {
                bool finished = false;
                while (!finished)
                {
                    finished = (Task.progress >= 0.9f || Task.isDone);

                    Debug.Log($"Progress: %{(Task.progress * 100):N2} ({completed + 1}/{levelLoadingData.LoadingSceneTasks.Count})");

                    if (finished)
                    {
                        completed++;
                        Task.allowSceneActivation = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.02f);
                    }
                }
            }

            bool cleanupFinished = false;
            while (!cleanupFinished)
            {
                float progress = 0;
                cleanupFinished = true;

                string txt = "";
                foreach (AsyncOperation Task in levelLoadingData.LoadingSceneTasks)
                {
                    progress += Task.isDone ? 1 : Task.progress;
                    cleanupFinished = cleanupFinished && Task.isDone;
                    txt = txt + Task.progress + " ";
                }

                progress /= levelLoadingData.LoadingSceneTasks.Count;
                Debug.Log($"Progress: %{(progress * 100):N2} -- {txt}");


                if (cleanupFinished)
                {
                    break;
                }
                else
                {
                    yield return new WaitForSeconds(0.02f);
                }
            }

            if (settings.LoadingFromMenu)
            {
                UnloadControlScene();
                ResetStats();
            }

            isLoadingLevel = false;
            OnLevelLoaded?.Invoke();
        }

        void ResetLoadedLevel()
        {
            if (RealTargetRespawnLevel != null && levelManager.CurrentLevel != RealTargetRespawnLevel)
            {
                LoadLevel(RealTargetRespawnLevel);
            }
        }

        public void ForceAtmosphere(GameObject postProcessingPrefab, bool instant)
        {
            levelManager.TransitionAtmosphere(postProcessingPrefab, instant);
        }
        #endregion

        #region Time Freezing
        private bool TimeFrozen
        {
            get
            {
                return Paused || Player.CurrentState == PlayerStates.Dead;
            }
        }

        private void UpdateTimeFreeze()
        {
            Time.timeScale = TimeFrozen? 0f : 1f;
        }
        #endregion

        #region Chapters
        public void LoadChapter(ChapterDescriptor newChapter)
        {
            StartingChapter = newChapter;
            UnityAction finishLoading = null;
            finishLoading = () =>
            {
                finishLoadingChapter(newChapter);
                OnLevelLoaded.RemoveListener(finishLoading);
            };
            OnLevelLoaded.AddListener(finishLoading);
            LoadLevel(newChapter.Level, new LevelLoadingSettings
            {
                InstantChangeAtmosphere = true,
                ForceReload = false,
                LoadingFromMenu = true
            });
        }

        private void finishLoadingChapter(ChapterDescriptor newChapter)
        {
            ChapterStartPoint respawnPoint = newChapter.FindStartPoint();
            respawnPoint.OnLoaded?.Invoke();
            RegisterRespawnPoint(respawnPoint.transform);
            ResetPlayer();
        }

#endregion

#region Equipment

        public NonInstancedEquipmentSlot[] Loadout;

        public UnityEvent<NonInstancedEquipmentSlot> OnEquip;
        public void Equip(EquipmentDescriptor equipment)
        {
            var slot = FindSlot(equipment.SlotType);

            if (slot == null)
            {
                Debug.LogWarning($"Tried to equip equipment {equipment} without its slot {equipment.SlotType}");
                return;
            }

            slot.Equipment = equipment;
            OnEquip?.Invoke(slot);
        }

        public NonInstancedEquipmentSlot FindSlot(EquipmentTypeDescriptor slotType)
        {
            foreach (var slot in Loadout)
            {
                if (slot.EquipmentType == slotType)
                {
                    return slot;
                }
            }

            return null;
        }
#endregion

#region Player
        private PlayerController player;
        public PlayerController Player => player;
        public static void RegisterPlayer(PlayerController player)
        {
            PogoInstance.player = player;
            player.OnStateChanged.AddListener((_) => PogoInstance.UpdateTimeFreeze());
        }

        public void KillPlayer(IKillType killType = null)
        {
            if (GameInstanceIsValid() && PogoInstance.player != null)
            {
                PogoInstance.player.Die(killType);
            }
        }

        public static void ResetPlayer()
        {
            if (GameInstanceIsValid() && PogoInstance.player != null)
            {
                PogoInstance.player.Reset();
            }
        }

        public UnityEvent OnPlayerDeath;
        public UnityEvent OnPlayerSpawn;
        #endregion

        #region Respawn Point

        public EventHandler OnCustomCheckpointChanged;

        public Transform CachedRespawnPoint;
        private Transform respawnPoint;
        public Transform RespawnPoint
        {
            get => respawnPoint; set
            {
                respawnPoint = value;
                if (respawnPoint != null && CachedRespawnPoint != null)
                {
                    CachedRespawnPoint.transform.position = respawnPoint.transform.position;
                    CachedRespawnPoint.transform.rotation = respawnPoint.transform.rotation;
                }
            }
        }

        public CustomCheckpointController CustomCheckpoint;
        public bool CustomRespawnActive;

        LevelDescriptor RespawnLevel;
        LevelDescriptor CustomRespawnLevel;
        public LevelDescriptor RealTargetRespawnLevel => CustomRespawnActive ? CustomRespawnLevel : RespawnLevel;

        public Transform GetRespawnTransform()
        {
            var point = CurrentDifficulty == Difficulty.Freeplay && CustomRespawnActive ? CustomCheckpoint.transform : RespawnPoint;
            return point == null ? CachedRespawnPoint : point;
        }

        public enum Difficulty
        {
            Normal,
            Hard,
            Freeplay,
            Expert,
            Challenge
        }
        public UnityEvent<DifficultyChangedEventArgs> OnDifficultyChanged;
        private Difficulty currentDifficulty = Difficulty.Normal;
        public Difficulty CurrentDifficulty
        {
            get => currentDifficulty;
            set
            {
                if (value == currentDifficulty) return;
                Debug.Log($"Changing difficulty {currentDifficulty} -> {value}");
                OnDifficultyChanged?.Invoke(new DifficultyChangedEventArgs(currentDifficulty, value));
                currentDifficulty = value;
            }
        }


        public bool RegisterCustomRespawnPoint(Vector3 point, Quaternion forward)
        {
            if (CurrentDifficulty == Difficulty.Freeplay && CustomCheckpoint.Place(point, forward))
            {
                CustomRespawnActive = true;
                CustomRespawnLevel = levelManager.CurrentLevel;
                OnCustomCheckpointChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        public bool ResetCustomRespawnPoint(bool force = false)
        {
            if (force || CurrentDifficulty == Difficulty.Freeplay && CustomRespawnActive)
            {
                CustomRespawnActive = false;
                CustomCheckpoint.Hide();
                OnCustomCheckpointChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        public bool TryRegisterRespawnPoint(Transform newRespawnPointTransform)
        {
            if (PogoInstance == null)
            {
                Debug.LogWarning("Tried to register a RespawnPoint with no CheckpointManager");
                return false;
            }

            if (!CanRegisterRespawnPoint(newRespawnPointTransform)) return false;

            RegisterRespawnPoint(newRespawnPointTransform);
            return true;
        }

        public void RegisterRespawnPoint(Transform newRespawnPointTransform)
        {
            RespawnLevel = levelManager.CurrentLevel;
            PogoInstance.RespawnPoint = newRespawnPointTransform;
        }

        public bool CanRegisterRespawnPoint(Transform newRespawnPointTransform)
        {
            if (newRespawnPointTransform == PogoInstance.RespawnPoint || CurrentDifficulty == Difficulty.Expert) return false;

            if (CurrentDifficulty == Difficulty.Normal || CurrentDifficulty == Difficulty.Freeplay)
            {
                return true;
            }

            var respawnPoint = newRespawnPointTransform.gameObject.GetComponent<RespawnPoint>();
            if (respawnPoint == null) return true;

            return (CurrentDifficulty == Difficulty.Hard && respawnPoint.EnabledInHardMode);
        }

        public override void LoadControlScene(ControlSceneDescriptor newScene, Action<List<AsyncOperation>> callback = null)
        {
            if (levelManager != null)
            {
                levelManager.ResetLoadedLevel();
            }
            base.LoadControlScene(newScene, callback);
        }

#endregion

#region Settings
        public static string KEY_FIELD_OF_VIEW = "FieldOfView";
        public static string KEY_SENSITIVITY = "Sensitivity";
        public static string KEY_INVERT = "InvertY";
        public static string KEY_TIMER = "ShowTimer";
        #endregion

        #region Stats
        public int SecretsFoundCount;
        public int NumberOfDeaths;
        public float GameStartTime;
        public ChapterDescriptor StartingChapter;

        public UnityEvent OnStatsReset;
        public void ResetStats()
        {
            SecretsFoundCount = 0;
            NumberOfDeaths = 0;
            GameStartTime = Time.time;
            OnStatsReset?.Invoke();
        }
#endregion
    }
}