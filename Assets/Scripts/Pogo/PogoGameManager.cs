using Assets.Scripts.Player;
using Inputter;
using Platforms;
using Pogo.Challenges;
using Pogo.Checkpoints;
using Pogo.Collectibles;
using Pogo.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
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

            LoadGlobalSave();
            RespawnPoint = new RespawnPointData(CachedRespawnPoint);
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
            OnQuitToMenu.AddListener(FullResetSessionData);
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
            list.Add(new GameSettingFloat(SETTINGKEY_GAMMA, 100));

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

        public void FullResetSessionData()
        {
            FinishChapter(false);
            SaveAndQuitSlot();
            SaveGlobalSave();
            ResetCustomRespawnPoint(true);
            ResetStats();
        }

        private void GameManager_OnQuitToDesktop()
        {
            FinishChapter(false);
            SaveAndQuitSlot();
            SaveGlobalSave();
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
                        yield return new WaitForSecondsRealtime(0.02f);
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
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }

            if (settings.LoadingFromMenu)
            {
                UnloadControlScene();
                ResetStats(settings.QuickSaveData);
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
        public bool CanSwitchChapters => CurrentDifficulty != Difficulty.Challenge;

        public void StartChapter(ChapterDescriptor chapter, QuickSaveData? quickSaveData = null)
        {
            if (CurrentChapter != null)
            {
                FinishChapter();
            }

            CurrentChapter = chapter;
            currentChapterProgressTracker = quickSaveData.HasValue
                ? new GameProgressTracker(this, quickSaveData.Value.ChapterProgressTimeMilliseconds, quickSaveData.Value.ChapterProgressDeaths)
                : new GameProgressTracker(this);

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

            if (CurrentSlotDataTracker != null)
            {
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
                else if (TrySerializeQuicksaveData(out QuickSaveData newData))
                {
                    CurrentSlotDataTracker.SlotData.quickSaveData = newData;
                }

                SetChapterSaveData(CurrentChapter, saveData);

                currentChapterProgressTracker = null;
            }

            this.CurrentChapter = null;
        }

        private bool TrySerializeQuicksaveData(out QuickSaveData newData)
        {
            if (CurrentCheckpoint == null)
            {
                Debug.LogWarning("Failed to quicksave. CurrentCheckpoint is missing!!!");
                newData = new QuickSaveData();
                return false;
            }

            if (CurrentCheckpoint.Descriptor == null)
            {
                Debug.LogWarning($"Failed to quicksave. CurrentCheckpoint {CurrentCheckpoint.name} no descriptor!!!", CurrentCheckpoint);
                newData = new QuickSaveData();
                return false;
            }

            if (CurrentCheckpoint.Descriptor.Chapter == null)
            {
                Debug.LogWarning($"Failed to quicksave. CurrentCheckpoint {CurrentCheckpoint.name} missing Chapter!!!", CurrentCheckpoint);
                newData = new QuickSaveData();
                return false;
            }

            if (CurrentCheckpoint.Descriptor.Level == null)
            {
                Debug.LogWarning($"Failed to quicksave. CurrentCheckpoint {CurrentCheckpoint.name} missing Level!!!", CurrentCheckpoint);
                newData = new QuickSaveData();
                return false;
            }

            newData = new QuickSaveData()
            {
                ChapterId = ChapterToId(CurrentCheckpoint.Descriptor.Chapter),
                checkpointId = CurrentCheckpoint.Descriptor.CheckpointId,
                CurrentState = QuickSaveData.States.InProgress,
                ChapterProgressDeaths = currentChapterProgressTracker?.TrackedDeaths ?? 0,
                SessionProgressDeaths = currentSessionProgressTracker.TrackedDeaths,
                ChapterProgressTimeMilliseconds = currentChapterProgressTracker?.TrackedTimeMilliseconds ?? 0,
                SessionProgressTimeMilliseconds = currentSessionProgressTracker.TrackedTimeMilliseconds
            };

            return true;
        }

        public WorldChapter FindChapter(ChapterId id)
        {
            if (id.WorldNumber != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return World.FindChapter(id.ChapterNumber);
        }

        public ChapterId ChapterToId(ChapterDescriptor chapter)
        {
            int index = World.IndexOf(chapter);
            if (index == -1) throw new KeyNotFoundException();

            return new ChapterId(0, index);
        }

        public void LoadChapter(ChapterDescriptor newChapter)
        {
            LoadChapter(newChapter, new CheckpointId(CheckpointTypes.MainPath, 1), null);
        }
        public void LoadChapter(ChapterDescriptor newChapter, CheckpointId checkpointId, QuickSaveData? quickSaveData)
        {
            LoadCheckpointManifest = new CheckpointManifest();
            CheckpointDescriptor checkpoint = newChapter.GetCheckpointDescriptor(checkpointId);
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
                LoadCheckpoint(checkpoint);
                StartChapter(newChapter, quickSaveData);
                OnLevelLoaded.RemoveListener(finishLoading);
            };
            OnLevelLoaded.AddListener(finishLoading);
            LoadLevel(checkpoint.Level, new LevelLoadingSettings
            {
                InstantChangeAtmosphere = true,
                ForceReload = false,
                LoadingFromMenu = CurrentControlScene != null,
                QuickSaveData = quickSaveData
            });
        }

        public void LoadQuickSave()
        {
            var chapter = FindChapter(CurrentSlotDataTracker.SlotData.quickSaveData.ChapterId);
            LoadChapter(chapter.Chapter, CurrentSlotDataTracker.SlotData.quickSaveData.checkpointId, CurrentSlotDataTracker.SlotData.quickSaveData);
            CurrentSlotDataTracker.RollbackQuicksaveProgress();
        }

        private void ShowChapterTitle(float delay = 0)
        {

            var titleInstance = UIManager.Instance.SpawnUIElement(ChapterTitleCardPrefab);
            titleInstance.GetComponent<TitleCardController>().DisplayTitle(CurrentChapter.Title, delay);
        }

        private void LoadCheckpoint(CheckpointDescriptor checkpointDescriptor)
        {
            bool checkpointFound = false;

            foreach (var checkpointTrigger in LoadCheckpointManifest.CheckpointTriggers)
            {
                checkpointTrigger.NotifyCheckpointLoad(checkpointDescriptor);
                if (!checkpointFound && checkpointTrigger.Descriptor == checkpointDescriptor)
                {
                    checkpointFound = true;
                    RegisterRespawnPoint(new RespawnPointData(checkpointTrigger));
                }
            }

            if (!checkpointFound)
            {
                throw new MissingReferenceException($"Failed to find Checkpoint trigger for {checkpointDescriptor}. did we load the right levels?");
            }

            CurrentGameState = GameStates.InGame;
            SpawnPlayer();
            LoadCheckpointManifest = null;
        }

        #endregion

        #region Checkpoint Shit
        private CheckpointManifest LoadCheckpointManifest;
        public CheckpointTrigger CurrentCheckpoint;

        public static void RegisterCheckpoint(CheckpointTrigger trigger)
        {
            if (PogoInstance == null) return;
            if (PogoInstance.LoadCheckpointManifest == null) return;

            PogoInstance.LoadCheckpointManifest.Add(trigger);

        }

        public bool TryGetNextCheckpoint(out CheckpointDescriptor nextCheckpoint)
        {
            // get the easy thing out of the way...
            if (!CurrentCheckpoint.Descriptor.CanSkip)
            {
                nextCheckpoint = null;
                return false;
            }

            if (CurrentCheckpoint.Descriptor.OverrideSkipToCheckpoint != null)
            {
                nextCheckpoint = CurrentCheckpoint.Descriptor.OverrideSkipToCheckpoint;
                return true;
            }
            else if (CurrentCheckpoint.Descriptor.CheckpointId.CheckpointType == CheckpointTypes.SidePath)
            {
#if UNITY_EDITOR
                Debug.LogError($"SidePath checkpoint has NO overrideSkipTarget but is marked as skippable!: {CurrentCheckpoint.Descriptor}");
#endif
                nextCheckpoint = null;
                return false;
            }


            // checkpoint numbers are one-indexed... for some fucking reason oh god why did I do that. so this looks weird
            if (CurrentCheckpoint.Descriptor.CheckpointId.CheckpointNumber + 1 > CurrentCheckpoint.Descriptor.Chapter.MainPathCheckpoints.Length)
            {
                // get the first checkpoint in the next chapter
                int chapterIndex = World.IndexOf(CurrentCheckpoint.Descriptor.Chapter);
                if (chapterIndex < 0)
                {
                    Debug.LogError($"Tried to skip out of a chapter... Couldn't find current Chapter {CurrentCheckpoint.Descriptor.Chapter} in current world {World}... ????");
                    nextCheckpoint = default;
                    return false;
                }

                WorldChapter nextChapter = World.FindChapter(chapterIndex + 1);
                if (nextChapter.Type != WorldChapter.Types.Level)
                {
                    Debug.LogError($"Tried to skip out of a chapter... next chapter (Index {chapterIndex+1}) is of bad type {nextChapter.Type}");
                    nextCheckpoint = default;
                    return false;
                }

                nextCheckpoint = nextChapter.Chapter.MainPathCheckpoints[0];
                if (nextCheckpoint == null)
                {
                    Debug.LogError($"Tried to skip out of a chapter... First checkpoint in next chapter is null for chapter {nextChapter.Chapter}");
                }
                return true;
            }


            nextCheckpoint = CurrentCheckpoint.Descriptor.Chapter.MainPathCheckpoints[CurrentCheckpoint.Descriptor.CheckpointId.CheckpointNumber + 1];
            return true;
        }

        public bool CanSkipCheckpoint()
        {
            if (CurrentCheckpoint == null) return false;

            switch (CurrentCheckpoint.SkipBehavior)
            {
                case CheckpointTrigger.SkipBehaviors.LevelChange:
                    return TrySkipCheckpointByLevelChange(true);
                case CheckpointTrigger.SkipBehaviors.TeleportToTarget:
                    return true;
                case CheckpointTrigger.SkipBehaviors.HalfCheckpoint:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException($"Checkpoint ({CurrentCheckpoint}) has bad SkipBehaviour {CurrentCheckpoint.SkipBehavior}");
            }
        }

        public bool TrySkipCheckpoint()
        {
            if (CurrentCheckpoint == null) return false;

            switch (CurrentCheckpoint.SkipBehavior)
            {
                case CheckpointTrigger.SkipBehaviors.LevelChange:
                    return TrySkipCheckpointByLevelChange();
                case CheckpointTrigger.SkipBehaviors.TeleportToTarget:
                    MovePlayerTo(CurrentCheckpoint.SkipTarget);
                    return true;
                case CheckpointTrigger.SkipBehaviors.HalfCheckpoint:
                    MovePlayerTo(CurrentCheckpoint.SkipTarget, true);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException($"Checkpoint ({CurrentCheckpoint}) has bad SkipBehaviour {CurrentCheckpoint.SkipBehavior}");
            }
        }

        private void MovePlayerTo(Transform targetLocation, bool alsoSetCustomCheckpoint = false)
        {
            if (alsoSetCustomCheckpoint)
            {
                RegisterCustomRespawnPoint(targetLocation.position, targetLocation.rotation.YawOnly());
            }

            Player.TeleportTo(targetLocation);
        }

        private bool TrySkipCheckpointByLevelChange(bool dry = false)
        {
            if (!TryGetNextCheckpoint(out CheckpointDescriptor nextCheckpoint))
            {
                return false;
            }

            // serialize quicksave data ourselves first, we're going to tweak this to load the next checkpoint
            if (!TrySerializeQuicksaveData(out QuickSaveData quickSaveData))
            {
                return false;
            }

            if (dry)
            {
                return true;
            }

            // Reset session data
            FullResetSessionData();

            // replace quicksavedata with the new stuff
            quickSaveData.ChapterId = new ChapterId(0, World.IndexOf(nextCheckpoint.Chapter));
            quickSaveData.checkpointId = nextCheckpoint.CheckpointId;
            CurrentSlotDataTracker.SlotData.quickSaveData = quickSaveData;

            // load quicksave data
            LoadQuickSave();

            return true;
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

        public void SpawnPlayer()
        {
            OnPlayerSpawn?.Invoke();
            Player.TeleportToSpawnpoint();
        }

        public void TrackDeath()
        {
            currentChapterProgressTracker?.TrackDeath();
            currentSessionProgressTracker?.TrackDeath();
            OnPlayerDeath?.Invoke();
        }

        public UnityEvent OnPlayerDeath;
        public UnityEvent OnPlayerSpawn;
        #endregion

        #region Respawn Point

        public EventHandler OnCustomCheckpointChanged;

        public Transform CachedRespawnPoint;
        private RespawnPointData respawnPoint;
        public RespawnPointData RespawnPoint
        {
            get => respawnPoint; set
            {
                respawnPoint = value;
                if (respawnPoint.transform != null && CachedRespawnPoint.transform != null)
                {
                    CachedRespawnPoint.position = respawnPoint.transform.position;
                    CachedRespawnPoint.rotation = respawnPoint.transform.rotation;
                }
                if (respawnPoint.Trigger != null)
                {
                    CurrentCheckpoint = respawnPoint.Trigger;
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
            var point = CurrentDifficulty == Difficulty.Freeplay && CustomRespawnActive ? CustomCheckpoint.transform : RespawnPoint.transform;
            return point == null ? CachedRespawnPoint.transform : point;
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

        public bool TryRegisterRespawnPoint(CheckpointTrigger trigger)
        {
            if (PogoInstance == null || PogoInstance.levelManager == null)
            {
                Debug.LogWarning("Tried to register a RespawnPoint with no CheckpointManager");
                return false;
            }

            if (!CanRegisterRespawnPoint(trigger.RespawnPoint)) return false;

            RegisterRespawnPoint(new RespawnPointData(trigger));
            return true;
        }

        public void RegisterRespawnPoint(RespawnPointData data)
        {
            RespawnLevel = data.OverrideLevel != null ? data.OverrideLevel : levelManager.CurrentLevel;
            PogoInstance.RespawnPoint = data;
        }

        public bool CanRegisterRespawnPoint(Transform newRespawnPointTransform)
        {
            if (newRespawnPointTransform == PogoInstance.RespawnPoint.transform || CurrentDifficulty == Difficulty.Expert) return false;

            if (CurrentDifficulty == Difficulty.Normal || CurrentDifficulty == Difficulty.Freeplay)
            {
                return true;
            }

            var respawnPoint = newRespawnPointTransform.gameObject.GetComponent<RespawnPoint>();
            if (respawnPoint == null) return true;

            return (CurrentDifficulty == Difficulty.Hard && respawnPoint.EnabledInHardMode);
        }

        public override void LoadControlScene(ControlSceneDescriptor newScene, Action callback = null)
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
        public static string SETTINGKEY_GAMMA = "Gamma";
        public static string SETTINGKEY_RESPAWNDELAY = "RespawnDelay";
        #endregion

        #region Stats
        private GameProgressTracker currentChapterProgressTracker;
        private GameProgressTracker currentSessionProgressTracker;

        public UnityEvent OnStatsReset;
        public void ResetStats()
        {
            ResetStats(null);
        }
        private void ResetStats(QuickSaveData? quickSaveData)
        {
            currentSessionProgressTracker = quickSaveData.HasValue
                ? new GameProgressTracker(this, quickSaveData.Value.SessionProgressTimeMilliseconds, quickSaveData.Value.SessionProgressDeaths)
                : new GameProgressTracker(this);
            OnStatsReset?.Invoke();
        }

        public int CurrentSessionDeaths => currentSessionProgressTracker?.TrackedDeaths ?? 0;

        public int TrackedSessionDeaths => currentSessionProgressTracker.TrackedDeaths;
        public TimeSpan TrackedSessionTime => currentSessionProgressTracker.TrackedTime;
        #endregion

        #region Saving
        [HideInInspector]
        public UnityEvent OnSaveSlotChanged;
        public SaveSlotDataTracker CurrentSlotDataTracker { get; private set; }
        public GlobalSaveDataTracker CurrentGlobalDataTracker { get; private set; }
        public ExplicitSaveSlotData EditorOverrideSlot3Data;
        public ExplicitGlobalSaveData EditorOverrideGlobalSaveData;
        
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
            SaveAndQuitSlot();

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

        public bool CurrentSlotHasValidQuicksaveData()
        {
            var data = CurrentSlotDataTracker.SlotData.quickSaveData;
            if (data.CurrentState == QuickSaveData.States.NoData)
            {
                return false;
            }

            try
            {
                if (data.ChapterId.WorldNumber != 0) throw new IndexOutOfRangeException();

                WorldChapter worldChapter = World.FindChapter(data.ChapterId.ChapterNumber);
                if (worldChapter.Type != WorldChapter.Types.Level)
                {
                    return false;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.LogError($"Failed to parse Quicksave data. {e}");
                return false;
            }

            var progressData = CurrentSlotDataTracker.GetChapterProgressData(data.ChapterId);
            if (!progressData.unlocked)
            {
                return false;
            }

            return true;
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


        public void SaveAndQuitSlot()
        {
            if (CurrentSlotDataTracker == null) return;

            CurrentSlotDataTracker.UpdatePreviewData(CollectibleManifest);
            CurrentSlotDataTracker.Save();
            CurrentSlotDataTracker = null;
        }

        public void LoadGlobalSave()
        {
            CurrentGlobalDataTracker = GetGlobalDataTracker();
            CurrentGlobalDataTracker.Load(true);
        }

        public void SaveGlobalSave()
        {
            if (CurrentGlobalDataTracker == null) return;

            CurrentGlobalDataTracker.UpdatePreviewData(CollectibleManifest);
            CurrentGlobalDataTracker.Save();
        }

        private GlobalSaveDataTracker GetGlobalDataTracker()
        {
#if DEBUG
            if (EditorOverrideGlobalSaveData != null)
            {
                return new ExplicitGlobalSaveDataTracker(EditorOverrideGlobalSaveData);
            }
#endif
            return new FileGlobalSaveDataTracker(PlatformService);
        }


        #endregion

        #region SoundManager
        public GlobalSoundManager SoundManager;

        public void PlayGlobalSound(GlobalSoundDescriptor sound)
        {
            SoundManager.Play(sound);
        }
        #endregion

        #region Collectibles
        public UnityEvent<CollectibleUnlockedEventArgs> OnCollectibleUnlocked;
        public GameObject GenericCollectibleNotificationPrefab;
        public CollectibleManifest CollectibleManifest;

        public void UnlockCollectible(CollectibleDescriptor collectible)
        {
            bool unlockedGlobally, unlockedInSlot;

            if (collectible.UnlockType != CollectibleDescriptor.UnlockTypes.SlotOnly)
            {
                unlockedGlobally = TryUnlockCollectibleGlobally(collectible);
            }
            else
            {
                unlockedGlobally = false;
            }

            if (collectible.UnlockType != CollectibleDescriptor.UnlockTypes.GlobalOnly)
            {
                unlockedInSlot = TryUnlockCollectibleInSlot(collectible);
            }
            else
            {
                unlockedInSlot = false;
            }



            OnCollectibleUnlocked?.Invoke(new CollectibleUnlockedEventArgs(collectible, unlockedGlobally, unlockedInSlot));
            if ((unlockedInSlot || unlockedGlobally))
            {
                SpawnCollectibleNotification(new CollectibleUnlockedEventArgs(collectible, unlockedGlobally, unlockedInSlot));
            }
        }

        private void SpawnCollectibleNotification(CollectibleUnlockedEventArgs args)
        {
            if (args.Collectible.NotificationPrefab != null)
            {
                _ = UIManager.Instance.SpawnUIElement(args.Collectible.NotificationPrefab);
            }
            else if (args.Collectible.SpawnGenericNotification)
            {
                var newElement = UIManager.Instance.SpawnUIElement(GenericCollectibleNotificationPrefab);
                newElement.GetComponent<GenericCollectibleNotificationController>().Initialize(args);
            }
        }

        private bool TryUnlockCollectibleGlobally(CollectibleDescriptor collectible)
        {
            CollectibleUnlockData slotData = CurrentGlobalDataTracker.GetCollectible(collectible.Key);
            if (slotData.isUnlocked)
            {
                return false;
            }

            slotData.isUnlocked = true;
            CurrentGlobalDataTracker.SetCollectible(slotData);
            return true;
        }

        private bool TryUnlockCollectibleInSlot(CollectibleDescriptor collectible)
        {
            CollectibleUnlockData slotData = CurrentSlotDataTracker.GetCollectible(collectible.Key);
            if (slotData.isUnlocked)
            {
                return false;
            }

            slotData.isUnlocked = true;
            CurrentSlotDataTracker.SetCollectible(slotData);
            return true;
        }
        #endregion
    }
}