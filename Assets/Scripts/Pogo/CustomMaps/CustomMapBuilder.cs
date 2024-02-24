using BSPImporter;
using BSPImporter.Textures;
using Pogo.Checkpoints;
using Pogo.CustomMaps.Entities;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.UI;
using Pogo.Difficulties;
using Pogo.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Pogo.CustomMaps
{
    public class CustomMapBuilder : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public LevelDescriptor CustomMapLevel;

        public PauseMenuController PauseMenu;
        public CustomMapClearedMenu MapClearedMenu;

        public CustomMap CurrentCustomMap;
        public Material DefaultMaterial;
        public EntityPrefabManifest EntityPrefabs;
        public MapAttemptData LastAttemptData;

        public KillTypeDescriptor[] KillTypes;

        private List<string> CustomMapRootPaths;
        private string WadFolderRootPath => $"{gameManager.PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}custom{Path.DirectorySeparatorChar}wads";
        private string BuiltInCustomFolder
        {
            get
            {
#if UNITY_EDITOR
                return $"{Application.dataPath}{Path.DirectorySeparatorChar}BuildEmbedRoot{Path.DirectorySeparatorChar}custom";
#else
                return $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}custom";
#endif
            }
        }

        public Dictionary<string, CustomMapEntityHandler> EntityHandlers { get; private set; }

        private void Start()
        {
            gameManager = PogoGameManager.PogoInstance;
            CustomMapRootPaths = new List<string>
            {
                $"{gameManager.PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}custom{Path.DirectorySeparatorChar}maps",
                $"{BuiltInCustomFolder}{Path.DirectorySeparatorChar}maps"
            };
        }

        public void LoadCustomMapLevel(string folderPath, string mapFileName)
        {
            if (EntityHandlers == null) SetupEntityHandlers();
            PogoGameManager pogoInstance = PogoGameManager.PogoInstance;
            pogoInstance.FullResetSessionData();
            pogoInstance.CurrentDifficulty = Difficulty.Normal;

            UnityAction finishLoading = null;
            finishLoading = () =>
            {
                SpawnCustomMap(folderPath, mapFileName);
                pogoInstance.OnLevelLoaded.RemoveListener(finishLoading);
            };
            pogoInstance.OnLevelLoaded.AddListener(finishLoading);
            pogoInstance.LoadLevel(new LevelLoadingSettings
            {
                Level = CustomMapLevel,
                MainLevelState = new LevelState(CustomMapLevel, 0),
                Instantly = true,
                ForceReload = true,
                LoadingFromMenu = true,
            });
        }

        private void SpawnCustomMap(string folderPath, string mapFileName)
        {
            CurrentCustomMap = new CustomMap();
            string fullMapPath = $"{folderPath}{Path.DirectorySeparatorChar}{mapFileName}";
            Debug.Log($"Tried to spawn customMap at path {fullMapPath} :D");
            BSPLoader.Settings settings = new()
            {
                path = fullMapPath,
                texturePath = folderPath,
                defaultMaterial = DefaultMaterial,
                meshCombineOptions = BSPLoader.MeshCombineOptions.PerEntity,
                assetSavingOptions = BSPLoader.AssetSavingOptions.None,
                curveTessellationLevel = 3,
                entityCreatedCallback = OnEntityCreated,
                scaleFactor = 1f / 32f
            };
            WadSource textureSource = new WadSource();


            // add built-in wad folder
            textureSource.AddWadFolder($"{BuiltInCustomFolder}{Path.DirectorySeparatorChar}wads");
            textureSource.AddWadFolder(WadFolderRootPath);
            textureSource.AddWadFolder(folderPath);
            var templateSource = new BSPImporter.EntityFactories.PrefabEntityFactory(GetEntityPrefabs());
            var loader = new BSPLoader(settings, textureSource, templateSource);

            loader.LoadBSP();
            SceneManager.MoveGameObjectToScene(loader.root, SceneManager.GetSceneByBuildIndex(CustomMapLevel.BuildIndex));

            if (CurrentCustomMap.FirstCheckpoint == null)
            {
                if (CurrentCustomMap.PlayerStart == null)
                {
                    throw new FormatException("Map contains no Main Path Checkpoints (an info_player_start works in a pinch!)");
                }
                else
                {
                    Debug.LogWarning("Map Contains no Main Path Checkpoints. Defaulting to an info_player_start!");
                }
                CurrentCustomMap.PlayerStart.AddComponent<BoxCollider>();
                var checkpoint = CurrentCustomMap.PlayerStart.AddComponent<GeneratedCheckpoint>();
                checkpoint.FixTriggerSettings();
                checkpoint.Id = new CheckpointId(CheckpointTypes.MainPath, 0);
                checkpoint.RespawnPoint = CurrentCustomMap.PlayerStart.transform;
                CurrentCustomMap.RegisterCheckpoint(checkpoint);
            }

            if (CurrentCustomMap.Finish != null)
            {
                CurrentCustomMap.Finish.OnActivated.AddListener(FinishMap);
            }

            foreach (var checkpoint in CurrentCustomMap.Checkpoints.Values)
            {
                FinishSettingUpTrigger_Checkpoint(checkpoint);
            }
            

            StartMap();
            gameManager.Paused = false;
        }

        public IEnumerable<MapHeader> GetMapHeaders()
        {
            return CustomMapRootPaths
                .SelectMany(path => Directory.GetDirectories(path))
                .Select(path => IndexingHelper.GenerateMapHeader(path, true))
                .Where(r => r.Success)
                .Select(r => r.Data);
        }

        public void RestartMap()
        {
            if (CurrentCustomMap == null) throw new InvalidOperationException("Tried to Restart with no Custom Map loaded");

            StartMap();
        }

        private void StartMap()
        {
            gameManager.ResetStats();
            gameManager.RegisterRespawnPoint(new RespawnPointData(CurrentCustomMap.FirstCheckpoint.RespawnPoint));
            PogoGameManager.PogoInstance.SpawnPlayer();
        }

        private void FinishMap()
        {
            LastAttemptData = new()
            {
                CompletionTimeMS = (int)gameManager.TrackedSessionTime.TotalMilliseconds,
                Deaths = gameManager.CurrentSessionDeaths
            };

            StartCoroutine(FinishMapRoutine());
        }

        private void DisposeCurrentMap()
        {
            if (CurrentCustomMap == null)
            {
                return;
            }

            if (CurrentCustomMap.SurfaceSource != null)
            {
                gameManager.MaterialSurfaceService.RemoveSource(CurrentCustomMap.SurfaceSource);
            }

            CurrentCustomMap = null;
        }

        private IEnumerator FinishMapRoutine()
        {
            yield return new WaitForSeconds(1);
            OpenMapClearedMenu();
        }

        private void OpenMapClearedMenu()
        {
            PauseMenu.OverrideMenu = MapClearedMenu;
            gameManager.Paused = true;
            PauseMenu.OverrideMenu = null;
        }

        private void OnEntityCreated(BSPLoader.EntityCreatedCallbackData data)
        {
            if (EntityHandlers.TryGetValue(data.Instance.entity.ClassName, out var handler))
            {
                handler.SetupAction.Invoke(data);
            }
        }


        #region Entity Prefabs
        private IEnumerable<KeyValuePair<string, GameObject>> GetEntityPrefabs()
        {
            foreach (var item in EntityPrefabs.Items)
            {
                yield return new KeyValuePair<string, GameObject>(item.ClassName, item.Prefab);
            }
        }
        #endregion

        #region Entity Handlers
        private void SetupEntityHandlers()
        {
            EntityHandlers = new Dictionary<string, CustomMapEntityHandler>();

            // we dont need to handle info_player_respawn. it's handled by trigger_checkpoint
            AddEntityHandler(new CustomMapEntityHandler("trigger_checkpoint", SetupTrigger_Checkpoint));
            AddEntityHandler(new CustomMapEntityHandler("trigger_finish", SetupTrigger_Finish));
            AddEntityHandler(new CustomMapEntityHandler("trigger_kill", SetupTrigger_Kill));
        }

        private void AddEntityHandler(CustomMapEntityHandler handler) => EntityHandlers.Add(handler.ClassName, handler);

        private void SetupTrigger_Checkpoint(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Checkpoint entity = new Trigger_Checkpoint(data);
            CheckpointId id = entity.GetCheckpointId();

            var target = entity.GetSingleTarget();

            var checkpoint = data.Instance.gameObject.GetComponent<GeneratedCheckpoint>();
            checkpoint.Id = id;
            checkpoint.RespawnPoint = target.gameObject.transform;
            checkpoint.CanSkip = entity.GetCanSkip();
            checkpoint.UpdateMesh();
            if (checkpoint.CanSkip && !string.IsNullOrEmpty(entity.GetOverrideSkipTargetName()))
            {
                var skipTarget = entity.GetSingleOverrideSkipTarget();
                checkpoint.SkipTarget = skipTarget.gameObject.transform;
            }

            var renderStyle = entity.GetRenderStyle();

            if (renderStyle == Trigger_Checkpoint.RenderStyles.Default)
            {
                checkpoint.GetComponent<Renderer>().material = checkpoint.DefaultMaterial;
            }
            else if (renderStyle == Trigger_Checkpoint.RenderStyles.Invisible)
            {
                checkpoint.GetComponent<Renderer>().enabled = false;
                checkpoint.Invisible = true;
            }

            try
            {
                CurrentCustomMap.RegisterCheckpoint(checkpoint);
            }
            catch (ArgumentException e)
            {
                throw new FormatException($"Map contains duplicate checkpoints with pathtype {id.CheckpointType} & number {id.CheckpointNumber}", e);
            }
        }

        private void SetupTrigger_Finish(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Finish entity = new Trigger_Finish(data);

            var checkpoint = data.Instance.gameObject.GetComponent<TriggerFinish>();
            checkpoint.UpdateMesh();

            var renderStyle = entity.GetRenderStyle();
            if (renderStyle == Trigger_Finish.RenderStyles.Default)
            {
                checkpoint.GetComponent<Renderer>().material = checkpoint.DefaultMaterial;
            }
            else if (renderStyle == Trigger_Finish.RenderStyles.Invisible)
            {
                checkpoint.GetComponent<Renderer>().enabled = false;
                checkpoint.Invisible = true;
            }

            CurrentCustomMap.RegisterFinish(checkpoint);
        }

        private void SetupTrigger_Kill(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Kill entity = new Trigger_Kill(data);

            var killTrigger = data.Instance.gameObject.GetComponent<KillTrigger>();
            killTrigger.DoExpensiveOriginStuff = true;

            int killTypeId = entity.GetKillTypeId();
            if (!ArrayHelper.ContainsIndex(KillTypes, killTypeId))
            {
                killTypeId = 0;
            }
            killTrigger.Type = KillTypes[killTypeId];

            var renderStyle = entity.GetRenderStyle();
            if (renderStyle == Trigger_Kill.RenderStyles.Default)
            {
                killTrigger.GetComponent<Renderer>().material = killTrigger.DefaultMaterial;
            }
            else if (renderStyle == Trigger_Kill.RenderStyles.Invisible)
            {
                killTrigger.GetComponent<Renderer>().enabled = false;
            }
        }
        #endregion

        private void FinishSettingUpTrigger_Checkpoint(GeneratedCheckpoint checkpoint)
        {
            if (checkpoint.CanSkip
                && checkpoint.Id.CheckpointType == CheckpointTypes.MainPath
                && CurrentCustomMap.Checkpoints.TryGetValue(checkpoint.Id + 1, out var nextCheckpoint))
            {
                checkpoint.SkipTarget = nextCheckpoint.RespawnPoint;
            }
        }
    }
}
