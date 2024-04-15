using Assets.Scripts.Player;
using Assets.Scripts.Pogo.Checkpoints;
using Inputter;
using Platforms;
using Pogo.Challenges;
using Pogo.Checkpoints;
using Pogo.Collectibles;
using Pogo.Cosmetics;
using Pogo.CustomMaps;
using Pogo.CustomMaps.Pickups;
using Pogo.CustomMaps.Steam;
using Pogo.Difficulties;
using Pogo.Levels;
using Pogo.MainMenu;
using Pogo.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using WizardPhysics.PhysicsTime;
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
#if !DISABLESTEAMWORKS
            CreateWorkshopUploadService();
#endif

            MaterialSurfaceService = new Surfaces.MaterialSurfaceService(DefaultSurfaceConfig)
                .AddSource(new Surfaces.AssetSurfaceSource(), 0);
            CurrentDifficultyDescriptor = DifficultyManifest.FindByKey(Difficulty.Normal);
            LoadCheckpointManifest = new CheckpointManifest();
            LoadGlobalSave();
            LoadCosmetics();
            RespawnPoint = new RespawnPointData(CachedRespawnPoint);
            levelManager = GetComponent<PogoLevelManager>();

#if UNITY_EDITOR
            if (InitialLevel != null)
            {
                LoadSlot(SaveSlotIds.Slot3);
                levelManager.SetCurrentLevelInEditor(InitialLevel);
                CurrentGameState = GameStates.InGame;
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
            LoadControlSceneAsync(MainMenuControlScene);
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (_CachedCheckpoint != null)
            {
                StartChapter(_CachedCheckpoint.Chapter);
                SetLevelState(_CachedCheckpoint.MainLevelState);
                if (_CachedCheckpoint.AdditionalLevelStates != null)
                {
                    foreach (var levelState in _CachedCheckpoint.AdditionalLevelStates)
                    {
                        TryInitializeLevelStateForLevel(levelState);
                    }
                }
            }
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
            list.Add(new GameSettingFloat(SETTINGKEY_SHOWRESTART, 0));
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

        public void FullResetSessionData() => FullResetSessionData(true);

        public void FullResetSessionData(bool quitSaveSlot = true)
        {
            FinishChapter(false);
            SaveSlot(quitAfterSave: quitSaveSlot);
            SaveGlobalSave();
            ResetCustomRespawnPoint(true);
            ResetStats();
            ResetLevelStates();
        }

        private void GameManager_OnQuitToDesktop()
        {
            FinishChapter(false);
            SaveSlot();
            SaveGlobalSave();
        }

        public ChallengeBuilder ChallengeBuilder;
        public CustomMapBuilder CustomMapBuilder;

        #region Main Menu Action
        private Action<PogoMainMenuController> OnMainMenuLoadAction;

        public bool TryGetMainMenuLoadAction(out Action<PogoMainMenuController> action)
        {
            action = OnMainMenuLoadAction;
            OnMainMenuLoadAction = null;
            return action != null;
        }

        public void DoMainMenuAction(Action<PogoMainMenuController> action)
        {
            var mainMenuScene = SceneManager.GetSceneByName("MainMenu");
            if (mainMenuScene.IsValid())
            {
                foreach(var rootObject in mainMenuScene.GetRootGameObjects())
                {
                    if (rootObject.TryGetComponent<PogoMainMenuController>(out var menu))
                    {
                        action(menu);
                        return;
                    }
                }
                throw new MissingComponentException();
            }
            else
            {
                OnMainMenuLoadAction = action;
            }
        }

        #endregion

        #region Surfaces
        public Surfaces.SurfaceConfig DefaultSurfaceConfig;

        [NonSerialized]
        public Surfaces.MaterialSurfaceService MaterialSurfaceService;
        #endregion

        #region Workshop
#if !DISABLESTEAMWORKS
        public WorkshopUploadService WorkshopUploadService;

        private void CreateWorkshopUploadService()
        {
            WorkshopUploadService = new WorkshopUploadService((Platforms.Steam.SteamPlatformService)PlatformService, PogoInstance);
        }
#endif

        #endregion

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

#if UNITY_EDITOR
        public override void LoadControlSceneInEditor(ControlSceneDescriptor newScene)
        {
            base.LoadControlSceneInEditor(newScene);
            var levelManager = GetComponent<PogoLevelManager>();

            levelManager.LoadDefaultAtmosphere();
            _CachedCheckpoint = null;
        }
#endif

        public void LoadLevel(LevelDescriptor newLevel) => LoadLevel(LevelLoadingSettings.DefaultWithLevel(newLevel));
        
        public void LoadLevel(LevelLoadingSettings settings)
        {
#if UNITY_EDITOR
            if (DontLoadScenesInEditor) return;
#endif
            settings.LoadingFromMenu = settings.LoadingFromMenu || CurrentControlScene != null;
            levelManager.LoadLevelAsync(settings);
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

        #region Level State

        [HideInInspector]
        public UnityEvent<LevelStateChangedArgs> OnLevelStateChanged;
        private Dictionary<LevelDescriptor, LevelState> CurrentLevelStates = new Dictionary<LevelDescriptor, LevelState>();

        [ContextMenu("Log Current LevelStates")]
        public void LogLevelStates()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Current LevelStates:");
            foreach(var item in CurrentLevelStates)
            {
                sb.AppendLine(item.Value.ToString());
            }

            Debug.Log(sb.ToString());
        }

        private void ResetLevelStates()
        {
            CurrentLevelStates.Clear();
        }

        public void TryInitializeLevelStateForLevel(LevelState levelState)
        {
            if (GetLevelStateForLevel(levelState.Level) == null)
            {
                SetLevelState(levelState, true);
            }
        }

        public void SetLevelState(LevelState newState, bool instant = false)
        {
            LevelStateChangedArgs args = new LevelStateChangedArgs(
                GetLevelStateForLevel(newState.Level),
                newState,
                instant
            );

            CurrentLevelStates[newState.Level] = newState;
            OnLevelStateChanged?.Invoke(args);
        }

        public bool TryGetLevelStateForLevel(LevelDescriptor levelDescriptor, out LevelState result) => CurrentLevelStates.TryGetValue(levelDescriptor, out result);

        public LevelState? GetLevelStateForLevel(LevelDescriptor levelDescriptor)
        {
            if (CurrentLevelStates.TryGetValue(levelDescriptor, out LevelState currentState))
            {
                return currentState;
            }
            else
            {
                return null;
            }
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

            if (CurrentCheckpoint is not ExplicitCheckpoint explicitCheckpoint)
            {
                throw new NotImplementedException();
            }

            if (explicitCheckpoint.Descriptor == null)
            {
                Debug.LogWarning($"Failed to quicksave. CurrentCheckpoint {explicitCheckpoint.name} no descriptor!!!", explicitCheckpoint);
                newData = new QuickSaveData();
                return false;
            }

            if (explicitCheckpoint.Descriptor.Chapter == null)
            {
                Debug.LogWarning($"Failed to quicksave. CurrentCheckpoint {explicitCheckpoint.name} missing Chapter!!!", explicitCheckpoint);
                newData = new QuickSaveData();
                return false;
            }

            if (explicitCheckpoint.Descriptor.MainLevelState.Level == null)
            {
                Debug.LogWarning($"Failed to quicksave. CurrentCheckpoint {explicitCheckpoint.name} missing Level!!!", explicitCheckpoint);
                newData = new QuickSaveData();
                return false;
            }

            newData = new QuickSaveData()
            {
                ChapterId = ChapterToId(explicitCheckpoint.Descriptor.Chapter),
                checkpointId = explicitCheckpoint.Descriptor.CheckpointId,
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
                LoadCheckpoint(checkpoint.Chapter, checkpoint.CheckpointId);
                StartChapter(newChapter, quickSaveData);
                OnLevelLoaded.RemoveListener(finishLoading);
            };
            OnLevelLoaded.AddListener(finishLoading);
            LoadLevel(new LevelLoadingSettings
            {
                Level = checkpoint.MainLevelState.Level,
                MainLevelState = checkpoint.MainLevelState,
                AdditionalDefaultLevelStates = checkpoint.AdditionalLevelStates,
                Instantly = true,
                ForceReload = true,
                LoadingFromMenu = true,
                QuickSaveData = quickSaveData
            });
        }

        public void LoadQuickSave()
        {
            var chapter = FindChapter(CurrentSlotDataTracker.SlotData.quickSaveData.ChapterId);
            LoadChapter(chapter.Chapter, CurrentSlotDataTracker.SlotData.quickSaveData.checkpointId, CurrentSlotDataTracker.SlotData.quickSaveData);
            CurrentSlotDataTracker.RollbackQuicksaveProgress();
        }

        public void ShowChapterTitle(float delaySeconds = 0)
        {
            ShowTextCrawl(CurrentChapter.Title, delaySeconds);
        }

        public void ShowTextCrawl(string text, float delaySeconds = 0)
        {
            var titleInstance = UIManager.Instance.SpawnUIElement(ChapterTitleCardPrefab);
            titleInstance.GetComponent<TitleCardController>().DisplayTitle(text, delaySeconds);
        }

        private void LoadCheckpoint(ChapterDescriptor chapter, CheckpointId checkpointId)
        {
            bool checkpointFound = false;

            foreach (ICheckpoint checkpoint in LoadCheckpointManifest.Checkpoints)
            {
                if (checkpoint.Chapter == chapter && checkpoint.Id == checkpointId)
                {
                    checkpointFound = true;
                    RegisterRespawnPoint(new RespawnPointData(checkpoint));
                    break;
                }
            }

            if (!checkpointFound)
            {
                throw new MissingReferenceException($"Failed to find Checkpoint trigger for {chapter} & {checkpointId}. did we load the right levels?");
            }

            CurrentGameState = GameStates.InGame;
            SpawnPlayer();
        }

        #endregion

        #region Checkpoint Shit
        public CheckpointManifest LoadCheckpointManifest { get; private set; }
        public ICheckpoint CurrentCheckpoint;
#if UNITY_EDITOR
        public CheckpointDescriptor _CachedCheckpoint;
#endif

        public bool TryGetNextCheckpoint(out CheckpointDescriptor nextCheckpoint)
        {
            if (this.CurrentCheckpoint is not ExplicitCheckpoint explicitCheckpoint)
            {
                throw new NotImplementedException();
            }

            // get the easy thing out of the way...
            if (!explicitCheckpoint.CanSkip)
            {
                nextCheckpoint = null;
                return false;
            }

            if (explicitCheckpoint.Descriptor.OverrideSkipToCheckpoint != null)
            {
                nextCheckpoint = explicitCheckpoint.Descriptor.OverrideSkipToCheckpoint;
                return true;
            }
            else if (explicitCheckpoint.Descriptor.CheckpointId.CheckpointType == CheckpointTypes.SidePath)
            {
#if UNITY_EDITOR
                Debug.LogError($"SidePath checkpoint has NO overrideSkipTarget but is marked as skippable!: {explicitCheckpoint.Descriptor}");
#endif
                nextCheckpoint = null;
                return false;
            }


            // checkpoint numbers are one-indexed... for some fucking reason oh god why did I do that. so this looks weird
            if (explicitCheckpoint.Descriptor.CheckpointId.CheckpointNumber + 1 > explicitCheckpoint.Descriptor.Chapter.MainPathCheckpoints.Length)
            {
                // get the first checkpoint in the next chapter
                int chapterIndex = World.IndexOf(explicitCheckpoint.Descriptor.Chapter);
                if (chapterIndex < 0)
                {
                    Debug.LogError($"Tried to skip out of a chapter... Couldn't find current Chapter {explicitCheckpoint.Descriptor.Chapter} in current world {World}... ????");
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

            // checkpoint numbers are one-indexed... for some fucking reason oh god why did I do that. so this looks weird
            nextCheckpoint = explicitCheckpoint.Descriptor.Chapter.MainPathCheckpoints[explicitCheckpoint.Descriptor.CheckpointId.CheckpointNumber + 0];
            return true;
        }

        public bool CanSkipCheckpoint()
        {
            if (this.CurrentCheckpoint is not ExplicitCheckpoint CurrentCheckpoint)
            {
                throw new NotImplementedException();
            }

            if (CurrentCheckpoint == null) return false;

            switch (CurrentCheckpoint.SkipBehavior)
            {
                case SkipBehaviors.LevelChange:
                    return TrySkipCheckpointByLevelChange(true);
                case SkipBehaviors.TeleportToTarget:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException($"Checkpoint ({CurrentCheckpoint}) has bad SkipBehaviour {CurrentCheckpoint.SkipBehavior}");
            }
        }

        public bool TrySkipCheckpoint()
        {
            if (this.CurrentCheckpoint is not ExplicitCheckpoint CurrentCheckpoint)
            {
                throw new NotImplementedException();
            }
            if (CurrentCheckpoint == null) return false;

            switch (CurrentCheckpoint.SkipBehavior)
            {
                case SkipBehaviors.LevelChange:
                    return TrySkipCheckpointByLevelChange();
                case SkipBehaviors.TeleportToTarget:
                    MovePlayerTo(CurrentCheckpoint.SkipTarget);
                    CurrentCheckpoint.OnSkip.Invoke();
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
            FullResetSessionData(quitSaveSlot: false);

            // replace quicksavedata with the new stuff
            quickSaveData.ChapterId = new ChapterId(0, World.IndexOf(nextCheckpoint.Chapter));
            quickSaveData.checkpointId = nextCheckpoint.CheckpointId;
            CurrentSlotDataTracker.SlotData.quickSaveData = quickSaveData;

            // load quicksave data
            LoadQuickSave();

            return true;
        }


        #endregion

        #region Cosmetics

        public CosmeticManifest CosmeticManifest;
        public GameObject GenericCosmeticNotificationPrefab;

        public void LoadCosmetics()
        {
            foreach(var manifest in CosmeticManifest.Slots)
            {
                var equipData = CurrentGlobalDataTracker.GetCosmeticSlotEquipData(manifest.Slot, manifest.Default.Key);
                CosmeticDescriptor descriptor = FindUnlockedCosmeticByKey(manifest, equipData.Key);
                    
                EquipCosmetic(descriptor, false);
            }
        }

        public CosmeticDescriptor GetCosmetic(CosmeticSlots slot, string defaultKey)
        {
            var equipData = CurrentGlobalDataTracker.GetCosmeticSlotEquipData(slot, defaultKey);
            CosmeticSlotManifest slotManifest = CosmeticManifest.Find(equipData.Slot);
            return FindUnlockedCosmeticByKey(slotManifest, equipData.Key);
        }

        public bool CosmeticIsUnlocked(CosmeticDescriptor cosmetic)
        {
            switch (cosmetic.UnlockType)
            {
                case CosmeticDescriptor.UnlockTypes.AlwaysUnlocked:
                    return true;
                case CosmeticDescriptor.UnlockTypes.VendingMachine:
                    return VendingCosmeticUnlocked(cosmetic);
                case CosmeticDescriptor.UnlockTypes.Collectible:
                    if (cosmetic.Collectible == null) return false;
                    return CurrentGlobalDataTracker.GetCollectible(cosmetic.Collectible.Key).isUnlocked;
                default:
                    return false;
            }
        }

        public bool VendingCosmeticUnlocked(CosmeticDescriptor cosmetic)
        {
            if (!CosmeticManifest.Vending.TryFind(cosmetic, out var vendingMachineEntry))
            {
                Debug.LogWarning($"Couldn't find Vending Cosmetic {cosmetic.Key}");
                return false;
            }

            return vendingMachineEntry.Cost <= CurrentGlobalDataTracker.SaveData.LastVendingMachineUnlock.Cost;
        }

        public bool TryGetNextVendingUnlock(out VendingMachineUnlockData result)
        {
            int lastCost = CurrentGlobalDataTracker.SaveData.LastVendingMachineUnlock.Cost;
            if (!CosmeticManifest.Vending.TryGetNext(lastCost, out VendingMachineEntry nextUnlock))
            {
                result = default;
                return false;
            }

            int coinsNeeded = nextUnlock.Cost - CurrentGlobalDataTracker.SaveData.CollectedCoins;
            result = new VendingMachineUnlockData()
            {
                Cosmetic = nextUnlock.Cosmetic,
                CoinsNeeded = coinsNeeded,
            };
            return true;
        }

        public void EquipCosmetic(CosmeticDescriptor cosmetic, bool saveChanges = true)
        {
            if (saveChanges)
            {
                CurrentGlobalDataTracker.SetCosmetic(new CosmeticEquipData()
                {
                    Slot = cosmetic.Slot,
                    Key = cosmetic.Key
                });
            }

            if (cosmetic.Equipment == null)
            {
                Debug.LogError($"Missing Equipment for Cosmetic {cosmetic}", cosmetic);
                return;
            }

            Equip(cosmetic.Equipment);
        }

        private CosmeticDescriptor FindUnlockedCosmeticByKey(CosmeticSlotManifest manifest, string key)
        {
            try
            {
                CosmeticDescriptor descriptor = manifest.FindByKey(key);

                if (CosmeticIsUnlocked(descriptor))
                {
                    return descriptor;
                }
                else
                {
                    return manifest.Default;
                }
            }
            catch (KeyNotFoundException)
            {
                return manifest.Default;
            }
        }

        public bool TryUnlockNextVendingUnlock(out CosmeticDescriptor unlockedCosmetic)
        {
            if (!TryGetNextVendingUnlock(out VendingMachineUnlockData unlockData)
                || unlockData.CoinsNeeded > 0)
            {
                unlockedCosmetic = default;
                return false;
            }

            if (unlockData.Cosmetic.UnlockType != CosmeticDescriptor.UnlockTypes.VendingMachine)
            {
                throw new ArgumentException($"Cosmetic {unlockData.Cosmetic.name} of bad type {unlockData.Cosmetic.UnlockType}");
            }

            if (!CosmeticManifest.Vending.TryFind(unlockData.Cosmetic, out var result))
            {
                Debug.LogError($"Tried to do vending machine stuff for a missing cosmetic {unlockData.Cosmetic}.");
                unlockedCosmetic = default;
                return false;
            }

            if (result.Cost >= CurrentGlobalDataTracker.SaveData.LastVendingMachineUnlock.Cost)
            {
                CurrentGlobalDataTracker.SaveData.LastVendingMachineUnlock = new VendingMachineLastUnlockSaveData()
                {
                    Cost = result.Cost,
                    Key = unlockData.Cosmetic.Key
                };
            }

            unlockedCosmetic = unlockData.Cosmetic;
            return true;
        }

        #endregion

        #region Equipment

        public NonInstancedEquipmentSlot[] Loadout;

        public UnityEvent<NonInstancedEquipmentSlot> OnEquip;
        public void Equip(EquipmentDescriptor equipment)
        {
            var slot = FindEquipmentSlot(equipment.SlotType);

            if (slot == null)
            {
                Debug.LogWarning($"Tried to equip equipment {equipment} without its slot {equipment.SlotType}");
                return;
            }

            slot.Equipment = equipment;
            OnEquip?.Invoke(slot);
        }

        public NonInstancedEquipmentSlot FindEquipmentSlot(EquipmentTypeDescriptor slotType)
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
            Player.Spawn();
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
        public UnityEvent<RespawnPointChangedEventArgs> OnRespawnPointChanged;
        public Transform CachedRespawnPoint;
        private RespawnPointData respawnPoint;
        public RespawnPointData RespawnPoint
        {
            get => respawnPoint; set
            {
                OnRespawnPointChanged?.Invoke(new RespawnPointChangedEventArgs()
                {
                    OldSpawnPoint = respawnPoint,
                    NewSpawnPoint = value
                });
                respawnPoint = value;
                if (respawnPoint.transform != null && CachedRespawnPoint.transform != null)
                {
                    CachedRespawnPoint.position = respawnPoint.transform.position;
                    CachedRespawnPoint.rotation = respawnPoint.transform.rotation;
                }
                if (respawnPoint.Checkpoint != null)
                {
                    CurrentCheckpoint = respawnPoint.Checkpoint;
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
            var point = CurrentDifficulty == Difficulty.Assist && CustomRespawnActive ? CustomCheckpoint.transform : RespawnPoint.transform;
            return point == null ? CachedRespawnPoint.transform : point;
        }

        public TeleportData GetRespawnTeleportData()
        {
            if (CurrentDifficulty == Difficulty.Assist && CustomRespawnActive)
            {
                return new TeleportData(CustomCheckpoint.transform, Vector3.zero);
            }
            else if (RespawnPoint.transform != null)
            {
                return new TeleportData(RespawnPoint.transform, RespawnPoint.InitialVelocity);                
            }
            else
            {
                return new TeleportData(CachedRespawnPoint.transform, Vector3.zero);
            }
        }

        public DifficultyManifest DifficultyManifest;
        public UnityEvent<DifficultyChangedEventArgs> OnDifficultyChanged;
        public DifficultyDescriptor CurrentDifficultyDescriptor { get; private set; }
        public Difficulty CurrentDifficulty
        {
            get => CurrentDifficultyDescriptor.DifficultyEnum;
            set
            {
                if (value == CurrentDifficultyDescriptor.DifficultyEnum) return;
                CurrentDifficultyDescriptor = DifficultyManifest.FindByKey(value);

                Debug.Log($"Changing difficulty {CurrentDifficulty} -> {value}");
                OnDifficultyChanged?.Invoke(new DifficultyChangedEventArgs(CurrentDifficulty, value));
            }
        }


        public bool RegisterCustomRespawnPoint(Vector3 point, Quaternion forward)
        {
            if (CurrentDifficulty == Difficulty.Assist && CustomCheckpoint.Place(point, forward))
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
            if (force || CurrentDifficulty == Difficulty.Assist && CustomRespawnActive)
            {
                CustomRespawnActive = false;
                CustomCheckpoint.Hide();
                OnCustomCheckpointChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        public bool TryRegisterRespawnPoint(Checkpoint checkpoint)
        {
            if (PogoInstance == null || PogoInstance.levelManager == null)
            {
                Debug.LogWarning("Tried to register a RespawnPoint with no CheckpointManager");
                return false;
            }

            if (!CanRegisterRespawnPoint(checkpoint.RespawnPoint)) return false;

            RegisterRespawnPoint(new RespawnPointData(checkpoint));
            return true;
        }

        public void RegisterRespawnPoint(RespawnPointData data)
        {
            RespawnLevel = data.OverrideLevel != null ? data.OverrideLevel : levelManager.CurrentLevel;
            PogoInstance.RespawnPoint = data;
        }

        public bool CanRegisterRespawnPoint(Transform newRespawnPointTransform)
        {
            if (newRespawnPointTransform == PogoInstance.RespawnPoint.transform) return false;

            if (CurrentDifficulty == Difficulty.Normal || CurrentDifficulty == Difficulty.Assist)
            {
                return true;
            }

            var respawnPoint = newRespawnPointTransform.gameObject.GetComponent<RespawnPoint>();
            if (respawnPoint == null) return true;

            return (CurrentDifficulty == Difficulty.Hard && respawnPoint.EnabledInHardMode);
        }

        public override void LoadControlSceneAsync(ControlSceneDescriptor newScene, Action callback = null)
        {
            if (levelManager != null)
            {
                levelManager.ResetLoadedLevel();
            }
            if (Player != null)
            {
                Player.CurrentState = PlayerStates.Alive;
            }
            base.LoadControlSceneAsync(newScene, callback);
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
        public static string SETTINGKEY_SHOWRESTART = "ShowRestart";
        #endregion

        #region Stats
        private GameProgressTracker currentChapterProgressTracker;
        private GameProgressTracker currentSessionProgressTracker;
        private bool hideStatsPopup;
        public bool HideStatsPopup
        {
            get => hideStatsPopup;
            set
            {
                hideStatsPopup = value;
                OnHideStatsChanged?.Invoke(value);
            }
        }

        public UnityEvent<bool> OnHideStatsChanged;
        public UnityEvent OnStatsReset;
        public void ResetStats()
        {
            ResetStats(null);
        }
        public void ResetStats(QuickSaveData? quickSaveData)
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

        private SaveSlotIds CurrentSlotId;
        private SaveSlotDataTracker currentSlotDataTracker;
        public SaveSlotDataTracker CurrentSlotDataTracker
        {
            get => currentSlotDataTracker; private set
            {
                currentSlotDataTracker = value;
            }
        }
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

        public void QuickRestart()
        {
            SaveSlotIds slotId = CurrentSlotId;
            DifficultyDescriptor difficulty = CurrentDifficultyDescriptor;
            string saveName = CurrentSlotDataTracker.PreviewData.name;
            FullResetSessionData();
            NewGameSlot(
                slotId,
                difficulty,
                saveName
                );
            LoadSlot(CurrentSlotId);
            LoadChapter(World.Chapters[0].Chapter);
        }

        public void LoadSlot(SaveSlotIds slotId)
        {
            SaveSlot();

            CurrentSlotDataTracker = GetSaveSlotTracker(slotId);
            CurrentSlotId = slotId;
            CurrentSlotDataTracker.Load();
            OnSaveSlotChanged?.Invoke();

            CurrentDifficulty = CurrentSlotDataTracker.PreviewData.difficulty;
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

                var checkpoint = worldChapter.Chapter.GetCheckpointDescriptor(data.checkpointId);
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

        public void SaveSlot(bool quitAfterSave = true)
        {
            if (CurrentSlotDataTracker == null) return;

            CurrentSlotDataTracker.UpdatePreviewData(CollectibleManifest, World);
            CurrentSlotDataTracker.Save();
            if (quitAfterSave)
            {
                CurrentSlotDataTracker = null;
            }
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
        public UnityEvent<PickupCollectedEventArgs> OnPickupCollected;

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
            if (args.Collectible.CollectibleType == CollectibleDescriptor.CollectibleTypes.Cosmetic)
            {
                var newElement = UIManager.Instance.SpawnUIElement(GenericCosmeticNotificationPrefab);
                newElement.GetComponent<GenericCosmeticNotificationController>().Initialize(args.Collectible.CosmeticDescriptor);
            }
            else if (args.Collectible.NotificationPrefab != null)
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

        #region Time Manager
        public PhysicsTimeManager TimeManager;


        #endregion
    }
}