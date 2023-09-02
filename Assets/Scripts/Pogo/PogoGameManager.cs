using Assets.Scripts.Player;
using Inputter;
using Platforms;
using Pogo.Challenges;
using Pogo.Checkpoints;
using Pogo.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.iOS;
using WizardUI;
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
                LoadSlot(SaveSlotIds.Slot3);
                levelManager.SetCurrentLevelInEditor(InitialLevel);
                currentSessionProgressTracker = new GameProgressTracker(this);
            }
#endif

            OnPauseStateChanged += ((_, _) => UpdateTimeFreeze());
            OnPlayerSpawn.AddListener(() => ResetLoadedLevel());
            OnQuitToMenu.AddListener(GameManager_OnQuitToMenu);
            OnQuitToDesktop.AddListener(GameManager_OnQuitToDesktop);
            CustomCheckpoint.OnPlaced.AddListener(() => OnCustomCheckpointChanged?.Invoke(this, EventArgs.Empty));
#if UNITY_EDITOR
#else
            LoadControlScene(MainMenuControlScene);       
#endif
        }

        protected override List<GameSettingFloat> LoadGameSettings()
        {
            var list = base.LoadGameSettings();

            list.Add(new GameSettingFloat(SETTINGKEY_FIELD_OF_VIEW, 90));
            list.Add(new GameSettingFloat(SETTINGKEY_SENSITIVITY, 0.1f));
            list.Add(new GameSettingFloat(SETTINGKEY_INVERT, 0f));
            list.Add(new GameSettingFloat(SETTINGKEY_TIMER, 0f));
            list.Add(new GameSettingFloat(SETTINGKEY_RESPAWNDELAY, 0.5f));

            return list;
        }

        protected override void Update()
        {
            base.Update();
            if (InputManager.CheckKeyDown(KeyName.Pause) && !InControlScene)
            {
                Paused = !Paused;
            }
        }

        private void GameManager_OnQuitToMenu()
        {
            FinishChapter(false);
            SaveSlot();
            ResetCustomRespawnPoint(true);
        }

        private void GameManager_OnQuitToDesktop()
        {
            FinishChapter(false);
            SaveSlot();
        }


        #region Level Management
        public enum GameStates
        {
            InMenu,
            InGame
        }
        public GameStates CurrentGameState { get; private set; }

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

            Action callBack = () =>
            {
                OnLoadLevelFinished(settings);
                OnLevelLoaded?.Invoke();
            };
            isLoadingLevel = true;

            settings.LoadingFromMenu = settings.LoadingFromMenu || CurrentControlScene != null;
#if UNITY_WEBGL
            if (!levelManager.LoadLevelAsync(newLevel, settings, (levelLoadingData) => StartCoroutine(loadLevelScenesInOrder(levelLoadingData, settings, callBack))))
#else
            if (!levelManager.LoadLevelAsync(newLevel, settings, (levelLoadingData) => StartCoroutine(loadLevelScenesSimultaneous(levelLoadingData, settings, callBack))))
#endif
            {
                isLoadingLevel = false;
            }
        }

        void OnLoadLevelFinished(LevelLoadingSettings settings)
        {
            LevelManager.TransitionAtmosphere(LevelManager.CurrentLevel, settings.InstantChangeAtmosphere);
        }

        IEnumerator loadLevelScenesSimultaneous(LevelLoadingData levelLoadingData, LevelLoadingSettings settings, Action callback = null)
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
            callback?.Invoke();
        }

        IEnumerator loadLevelScenesInOrder(LevelLoadingData levelLoadingData, LevelLoadingSettings settings, Action callback = null)
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
            callback?.Invoke();
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
        public WorldDescriptor World;
        private ChapterDescriptor currentChapter;
        public GameObject ChapterTitleCardPrefab;
        public ChapterDescriptor CurrentChapter
        {
            get => currentChapter;
            private set => currentChapter = value;
        }
        public bool CanSwitchChapters => true;

        public void StartChapter(ChapterDescriptor chapter)
        {
            if (CurrentChapter != null)
            {
                FinishChapter();
            }

            CurrentChapter = chapter;
            currentChapterProgressTracker = new GameProgressTracker(this);
            ChapterSaveData saveData = GetChapterSaveData(chapter);
            if (!saveData.unlocked)
            {
                saveData.unlocked = true;
                SetChapterSaveData(chapter, saveData);
            }

            if (CurrentGameState == GameStates.InGame)
            {
                ShowChapterTitle(0.5f);
            }
        }

        public void FinishChapter(bool markComplete = true)
        {
            if (CurrentChapter == null) return;

            ChapterSaveData saveData = GetChapterSaveData(CurrentChapter);

            saveData.deathsTracked += currentChapterProgressTracker.TrackedDeaths;
            saveData.millisecondsElapsed += currentChapterProgressTracker.TrackedTimeMilliseconds;

            if (markComplete)
            {
                saveData.bestDeaths = !saveData.complete
                    ? currentChapterProgressTracker.TrackedDeaths
                    : Math.Min(saveData.bestDeaths, currentChapterProgressTracker.TrackedDeaths);
                saveData.millisecondsBestTime = !saveData.complete
                    ? currentChapterProgressTracker.TrackedTimeMilliseconds
                    : Math.Min(saveData.millisecondsBestTime, currentChapterProgressTracker.TrackedTimeMilliseconds);
                saveData.complete = true;
            }

            SetChapterSaveData(CurrentChapter, saveData);

            this.CurrentChapter = null;
            currentChapterProgressTracker = null;
        }

        public void LoadChapter(ChapterDescriptor newChapter)
        {
            LoadChapter(newChapter, new CheckpointId(CheckpointTypes.MainPath, 1));
        }
        public void LoadChapter(ChapterDescriptor newChapter, CheckpointId checkpointId)
        {
            var checkpoint = newChapter.GetCheckpointDescriptor(checkpointId);
            if (checkpoint == null)
            {
                if (checkpointId.CheckpointType == CheckpointTypes.MainPath
                    && checkpointId.CheckpointNumber == 1)
                {
                    throw new NullReferenceException($"Chapter {newChapter} Defines no checkpoints");
                }
                else
                {
                    Debug.LogError($"Failed to find checkpoint {checkpointId}. Defaulting to first checkpoint.");
                    LoadChapter(newChapter);
                    return;
                }
            }

            UnityAction finishLoading = null;
            finishLoading = () =>
            {
                LoadCheckpoint(newChapter, checkpoint);
                StartChapter(newChapter);
                OnLevelLoaded.RemoveListener(finishLoading);
            };
            OnLevelLoaded.AddListener(finishLoading);
            LoadLevel(checkpoint.Level, new LevelLoadingSettings
            {
                InstantChangeAtmosphere = true,
                ForceReload = false,
                LoadingFromMenu = true
            });
        }

        private void ShowChapterTitle(float delay = 0)
        {

            var titleInstance = UIManager.Instance.SpawnUIElement(ChapterTitleCardPrefab);
            titleInstance.GetComponent<TitleCardController>().DisplayTitle(CurrentChapter.Title, delay);
        }

        private void LoadCheckpoint(ChapterDescriptor chapter, CheckpointDescriptor checkpointDescriptor)
        {
            CheckpointLoadEventArgs checkpointLoadArgs = new(chapter, checkpointDescriptor);
            bool checkpointFound = false;

            var checkpointTriggers = FindObjectsOfType(typeof(CheckpointTrigger)) as CheckpointTrigger[];


            foreach (var checkpointTrigger in checkpointTriggers)
            {
                checkpointTrigger.NotifyCheckpointLoaded(checkpointLoadArgs);
                if (!checkpointFound && checkpointTrigger.Descriptor == checkpointDescriptor)
                {
                    checkpointFound = true;
                    RegisterRespawnPoint(checkpointTrigger.RespawnPoint);
                }
            }

            if (!checkpointFound)
            {
                throw new MissingReferenceException($"Failed to find Checkpoint trigger for {checkpointDescriptor}. did we load the right levels?");
            }

            CurrentGameState = GameStates.InGame;
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

        public void KillPlayer(PlayerDeathData data)
        {
            if (GameInstanceIsValid() && PogoInstance.player != null)
            {
                PogoInstance.player.Die(data);
            }
        }

        public void ResetPlayer()
        {
            if (GameInstanceIsValid() && PogoInstance.player != null)
            {
                PogoInstance.player.Respawn();
            }
        }

        public void KillPlayer()
        {
            if (GameInstanceIsValid() && PogoInstance.player != null)
            {
                PogoInstance.player.Die();
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

        public DifficultyManifest DifficultyManifest;
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
            if (Player != null)
            {
                Player.CurrentState = PlayerStates.Alive;
            }
            base.LoadControlScene(newScene, callback);
            CurrentGameState = GameStates.InMenu;
        }

        #endregion

        #region Final Score
        public UnityEvent OnStoreFinalStats;
        public static float FinalTime;
        #endregion

        #region Settings
        public static string SETTINGKEY_FIELD_OF_VIEW = "FieldOfView";
        public static string SETTINGKEY_SENSITIVITY = "Sensitivity";
        public static string SETTINGKEY_INVERT = "InvertY";
        public static string SETTINGKEY_TIMER = "ShowTimer";
        public static string SETTINGKEY_RESPAWNDELAY = "RespawnDelay";
        #endregion

        #region Stats
        private GameProgressTracker currentChapterProgressTracker;
        private GameProgressTracker currentSessionProgressTracker;

        public UnityEvent OnStatsReset;
        public void ResetStats()
        {
            currentSessionProgressTracker = new GameProgressTracker(this);
            OnStatsReset?.Invoke();
        }

        public int TrackedSessionDeaths => currentSessionProgressTracker.TrackedDeaths;
        public TimeSpan TrackedSessionTime => currentSessionProgressTracker.TrackedTime;
        #endregion

        #region Saving
        [HideInInspector]
        public UnityEvent OnSaveSlotChanged;
        private SaveSlotDataTracker CurrentSlotDataTracker;
        public ExplicitSaveSlotData EditorOverrideSlot3Data;
        
        public SaveSlotDataTracker PreviewSlot(SaveSlotIds slotId)
        {
            SaveSlotDataTracker tracker = GetSaveSlotTracker(slotId);
            tracker.Load();
            return tracker;
        }

        public void DeleteSlot(SaveSlotIds slotId)
        {
            var tracker = GetSaveSlotTracker(slotId);
            tracker.Delete();
        }

        public void LoadSlot(SaveSlotIds slotId)
        {
            SaveSlot();

            CurrentSlotDataTracker = GetSaveSlotTracker(slotId);
            CurrentSlotDataTracker.Load();
            OnSaveSlotChanged?.Invoke();

            var difficultyId = CurrentSlotDataTracker.PreviewData.difficulty;
            var difficulty = DifficultyManifest.FindByKey(difficultyId);
            Equip(difficulty.PogoEquipment);
        }

        public void NewGameSlot(
            SaveSlotIds slotId,
            DifficultyDescriptor difficulty,
            string Name)
        {
            var tracker = GetSaveSlotTracker(slotId);
            tracker.InitializeNew(Name, difficulty.DifficultyEnum);
            tracker.Save();
        }

        private SaveSlotDataTracker GetSaveSlotTracker(SaveSlotIds slotId)
        {
#if DEBUG
            if (slotId == SaveSlotIds.Slot3
                && EditorOverrideSlot3Data != null)
            {
                return new ExplicitSaveSlotDataTracker(EditorOverrideSlot3Data);
            }
#endif

            return new FileSaveSlotDataTracker(PlatformService, "saveslot", slotId);
        }

        public QuickSaveData GetQuickSaveData()
        {
            return CurrentSlotDataTracker.SlotData.quickSaveData;
        }

        public ChapterSaveData GetChapterSaveData(ChapterDescriptor chapter)
        {
            int index = World.IndexOf(chapter);
            if (index == -1) throw new KeyNotFoundException($"Couldn't find chapter {chapter} in world {World}");

            ChapterId id = new ChapterId(0, index);
            
            return CurrentSlotDataTracker.GetChapterProgressData(id);
        }

        public void SetChapterSaveData(ChapterDescriptor chapter, ChapterSaveData data)
        {
            int index = World.IndexOf(chapter);

            if (index == -1) throw new KeyNotFoundException($"Couldn't find chapter {chapter} in world {World}");

            ChapterId id = new ChapterId(0, index);

            CurrentSlotDataTracker.SetChapterProgressData(id, data);
        }

        public void UnlockChapter(ChapterDescriptor chapter)
        {
            int index = World.IndexOf(chapter);
            if (index == -1) throw new KeyNotFoundException($"Couldn't find chapter {chapter} in world {World}");

            ChapterId id = new ChapterId(0, index);

            ChapterSaveData data = CurrentSlotDataTracker.GetChapterProgressData(id);
            data.unlocked = true;

            CurrentSlotDataTracker.SetChapterProgressData(id, data);
        }

        public void SetQuickSaveData(QuickSaveData data)
        {
            CurrentSlotDataTracker.SlotData.quickSaveData = data;
        }

        public void SaveSlot()
        {
            if (CurrentSlotDataTracker == null) return;

            CurrentSlotDataTracker.UpdatePreviewData();
            CurrentSlotDataTracker.Save();
        }

        #endregion

        #region SoundManager
        public GlobalSoundManager SoundManager;

        public void PlayGlobalSound(GlobalSoundDescriptor sound)
        {
            SoundManager.Play(sound);
        }
        #endregion
    }
}