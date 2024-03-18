using Assets.Scripts.Pogo.CustomMaps;
using BSPImporter;
using BSPImporter.Textures;
using Pogo.Checkpoints;
using Pogo.CustomMaps.Entities;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.MapSources;
using Pogo.CustomMaps.Materials;
using Pogo.CustomMaps.UI;
using Pogo.Difficulties;
using Pogo.Levels;
using Pogo.Surfaces;
using Pogo.Trains;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        public Material DefaultSkyMaterial;

        public EntityPrefabManifest EntityPrefabs;
        public Materials.FillableShaderManifest FillableShaderManifest;
        public Materials.StockMaterialManifest StockMaterialManifest;
        public MapAttemptData LastAttemptData;
        public GameObject InfoCameraPreviewPrefab;

        public Dictionary<string, SurfaceConfig> SurfaceConfigDictionary;
        public SurfaceConfig DefaultSurfaceConfig;

        public KillTypeDescriptor[] KillTypes;

        public string WadFolderRootPath => $"{gameManager.PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}custom{Path.DirectorySeparatorChar}wads";
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
            gameManager.OnControlSceneChanged += GameManager_OnControlSceneChanged;
            SurfaceConfigDictionary = new Dictionary<string, SurfaceConfig>();
            foreach (var config in Resources.LoadAll<SurfaceConfig>("Surfaces"))
            {
                if (string.IsNullOrWhiteSpace(config.WadKey)) continue;

                SurfaceConfigDictionary.Add(config.WadKey, config);
            }
        }

        private void GameManager_OnControlSceneChanged(object sender, ControlSceneEventArgs e)
        {
            DisposeCurrentMap();
        }

        private IEnumerable<IMapSource> GetMapSources()
        {
            return new IMapSource[]
            {
                new MapSourceFolder(true, $"{gameManager.PlatformService.PersistentDataPath}{Path.DirectorySeparatorChar}custom{Path.DirectorySeparatorChar}maps"),
                new MapSourceFolder(false, $"{BuiltInCustomFolder}{Path.DirectorySeparatorChar}maps"),
#if !DISABLESTEAMWORKS
                new Steam.WorkshopSubscriptionSource()
#endif
            };
        }

        public void LoadCustomMapLevel(MapHeader header, Action callback = null)
        {
            if (EntityHandlers == null) SetupEntityHandlers();
            PogoGameManager pogoInstance = PogoGameManager.PogoInstance;
            pogoInstance.FullResetSessionData();
            pogoInstance.CurrentDifficulty = Difficulty.Normal;

            UnityAction finishLoading = null;
            finishLoading = () =>
            {
                SpawnCustomMap(header);
                callback?.Invoke();
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

        public void LoadMapAndGenerateThumbnail(MapHeader header, Action<GenerateMapThumbnailResult> callback)
        {
            Debug.Log($"Loading Map {header.MapName} to generate thumbnail...");
            LoadCustomMapLevel(header, () =>
            {
                Debug.Log($"generating thumbnail...");
                var result = GenerateThumbnailForCurrentMap();
                callback?.Invoke(result);
            });
        }

        private GenerateMapThumbnailResult GenerateThumbnailForCurrentMap()
        {
            if (CurrentCustomMap.InfoCameraThumbnailObject == null)
            {
                return new GenerateMapThumbnailResult()
                {
                    MapHeader = CurrentCustomMap.Header,
                    ResultType = GenerateMapThumbnailResult.ResultTypes.FailureMissingEntity
                };
            }

            var newCamera = Instantiate(InfoCameraPreviewPrefab, CurrentCustomMap.InfoCameraThumbnailObject.transform)
                .GetComponent<CustomMapIconGenerator>();

            Sprite sprite = newCamera.RenderToSprite();
            MapHeaderHelper.SaveThumbnail(CurrentCustomMap.Header, sprite);
            return new GenerateMapThumbnailResult()
            {
                MapHeader = CurrentCustomMap.Header,
                ResultType = GenerateMapThumbnailResult.ResultTypes.Success
            };
        }

        private void SpawnCustomMap(MapHeader header)
        {
            string folderPath = header.FolderPath;
            CurrentCustomMap = new CustomMap()
            {
                Header = header,
            };
            string fullMapPath = $"{folderPath}{Path.DirectorySeparatorChar}{header.MapName}.bsp";
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
            textureSource.AddWadFolder($"{BuiltInCustomFolder}{Path.DirectorySeparatorChar}wads");
            textureSource.AddWadFolder(WadFolderRootPath);
            textureSource.AddWadFolder(folderPath);
            textureSource.OnTextureNotFound += TextureSource_OnTextureNotFound;

            var templateSource = new BSPImporter.EntityFactories.PrefabEntityFactory(GetEntityPrefabs());

            var materialSource = new Materials.PogoMaterialSource(
                DefaultMaterial,
                FillableShaderManifest.Items,
                StockMaterialManifest.Items);

            // set up atmosphere from header
            var newAtmo = Instantiate(CustomMapLevel.PostProcessingPrefab).GetComponent<Atmospheres.Atmosphere>();
            newAtmo.FogDensity = header.FogThicknessOrDefault();
            newAtmo.FogColor = header.FogColorOrDefault();
            gameManager.LevelManager.OverrideAtmosphere(newAtmo, true);

            // set up sky texture from header
            RegisterSkyTexture(header.SkyTexture, textureSource, materialSource, DefaultSkyMaterial);

            var loader = new BSPLoader(settings, textureSource, templateSource, materialSource);

            try
            {
                loader.LoadBSP();
            }
            catch(Exception ex)
            {
                CurrentCustomMap.AddError(new Errors.MapError(
                    ex,
                    "Load Error - See Exception",
                    Errors.MapError.Severities.Error));
            }
            SceneManager.MoveGameObjectToScene(loader.root, SceneManager.GetSceneByBuildIndex(CustomMapLevel.BuildIndex));
            SetupMapSurfaceSource(loader);
            gameManager.MaterialSurfaceService.AddSource(CurrentCustomMap.SurfaceSource, -1);

            if (CurrentCustomMap.FirstCheckpoint == null)
            {
                if (CurrentCustomMap.PlayerStart == null)
                {
                    CurrentCustomMap.AddError(new Errors.MapError(
                        null,
                        "Map contains no Main Path Checkpoints (an info_player_start works in a pinch!)",
                        Errors.MapError.Severities.Error));
                }
                else
                {
                    CurrentCustomMap.AddError(new Errors.MapError(
                        null,
                        "Map Contains no Main Path Checkpoints. Defaulting to an info_player_start!",
                        Errors.MapError.Severities.Warning));

                    CurrentCustomMap.PlayerStart.AddComponent<BoxCollider>();
                    var checkpoint = CurrentCustomMap.PlayerStart.AddComponent<GeneratedCheckpoint>();
                    checkpoint.FixTriggerSettings();
                    checkpoint.Id = new CheckpointId(CheckpointTypes.MainPath, 0);
                    checkpoint.RespawnPoint = CurrentCustomMap.PlayerStart.transform;
                    CurrentCustomMap.RegisterCheckpoint(checkpoint);
                }
            }

            if (!CurrentCustomMap.HasFinish)
            {
                CurrentCustomMap.AddError(new Errors.MapError(
                    null,
                    "Map contains no Trigger_Finish. There's no way to win :(",
                    Errors.MapError.Severities.Warning));
            }

            WriteMapLoadLogs(gameManager, CurrentCustomMap);
            foreach (var checkpoint in CurrentCustomMap.Checkpoints.Values)
            {
                FinishSettingUpTrigger_Checkpoint(checkpoint);
            }

            if (CurrentCustomMap.HasError)
            {
                ReturnToMainMenuAndShowErrors();
                return;
            }

            StartMap();
            gameManager.Paused = false;
        }

        private static void RegisterSkyTexture(
            string textureName,
            WadSource textureSource,
            PogoMaterialSource materialSource,
            Material fallbackMaterial)
        {
            Material skyMaterial;

            if (textureName != null)
            {
                var wadTexture = textureSource.LoadTexture(textureName);
                if (wadTexture != null)
                {
                    skyMaterial = materialSource.BuildMaterial(wadTexture.Value);
                }
                else
                {
                    Debug.LogWarning($"Failed to load SkyTexture named '{textureName}'");
                    skyMaterial = fallbackMaterial;
                }
            }
            else
            {
                skyMaterial = fallbackMaterial;
            }

            materialSource.RegisterMaterial("tool_skybox", skyMaterial);
        }

        public IEnumerable<MapHeader> GetMapHeaders(bool uploadableOnly = false)
        {
            IEnumerable<IMapSource> sources = GetMapSources();
            if (uploadableOnly)
            {
                sources = sources
                    .Where(s => s.AllowUpload);
            }

            return sources
                .SelectMany(s => s.GetPaths())
                .Select(path => MapHeaderHelper.GenerateMapHeader(path, true))
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
            yield return new WaitForSeconds(0.4f);
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
                try
                {
                    handler.SetupAction.Invoke(data);
                }
                catch (Exception e)
                {
                    CurrentCustomMap.AddError(new Errors.CreateEntityError(e, data));
                }
            }
        }

        private void SetupMapSurfaceSource(BSPLoader loader)
        {
            foreach(var kv in loader.materialDirectory)
            {
                SurfaceConfig surfaceConfig;
                string key = kv.Value.GetSurfaceType();
                if (!string.IsNullOrEmpty(key))
                {
                    surfaceConfig = SurfaceConfigDictionary.GetValueOrDefault(key, DefaultSurfaceConfig);
                }
                else
                {
                    surfaceConfig = DefaultSurfaceConfig;
                }
                CurrentCustomMap.SurfaceSource.RegisterMaterial(kv.Value.Material, surfaceConfig);
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
            AddEntityHandler(new CustomMapEntityHandler("func_breakable", SetupFunc_Breakable));
            AddEntityHandler(new CustomMapEntityHandler("func_illusionary", SetupFunc_Illusionary));
            AddEntityHandler(new CustomMapEntityHandler("func_invisible", SetupFunc_Invisible));
            AddEntityHandler(new CustomMapEntityHandler("func_train", SetupFunc_Train));
            AddEntityHandler(new CustomMapEntityHandler("trigger_checkpoint", SetupTrigger_Checkpoint));
            AddEntityHandler(new CustomMapEntityHandler("trigger_finish", SetupTrigger_Finish));
            AddEntityHandler(new CustomMapEntityHandler("trigger_kill", SetupTrigger_Kill));
            AddEntityHandler(new CustomMapEntityHandler("trigger_gravity", SetupTrigger_Gravity));
            AddEntityHandler(new CustomMapEntityHandler("trigger_flight", SetupTrigger_Flight));
            AddEntityHandler(new CustomMapEntityHandler("trigger_teleport", SetupTrigger_Teleport));
            AddEntityHandler(new CustomMapEntityHandler("info_camera_preview", SetupInfo_Camera_Preview));
        }

        private void AddEntityHandler(CustomMapEntityHandler handler) => EntityHandlers.Add(handler.ClassName, handler);

        private void SetupInfo_Camera_Preview(BSPLoader.EntityCreatedCallbackData data)
        {
            CurrentCustomMap.InfoCameraThumbnailObject = data.Instance.gameObject;
        }

        private void SetupFunc_Breakable(BSPLoader.EntityCreatedCallbackData data)
        {
            Func_Breakable self = new Func_Breakable(data.Instance, data.Context);

            var breakable = data.Instance.gameObject.GetComponent<Gimmicks.FuncBreakable>();
            breakable.UpdateMesh();
            breakable.RegenerateOnPlayerSpawn = self.GetRegenOnPlayerSpawn();
        }

        private void SetupFunc_Illusionary(BSPLoader.EntityCreatedCallbackData data)
        {
            data.Instance.gameObject.GetComponent<MeshCollider>().enabled = false;
        }

        private void SetupFunc_Invisible(BSPLoader.EntityCreatedCallbackData data)
        {
            data.Instance.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        private void SetupFunc_Train(BSPLoader.EntityCreatedCallbackData data)
        {
            Func_Train entity = new Func_Train(data.Instance, data.Context);

            var root = data.Instance.gameObject.GetComponent<FuncTrainRoot>();
            BSPLoader.EntityInstance trackStart = entity.GetTrackStart();
            string surfaceName = entity.GetSurface();
            SurfaceConfig surface = string.IsNullOrEmpty(surfaceName)
                ? DefaultSurfaceConfig
                : SurfaceConfigDictionary.GetValueOrDefault(surfaceName, DefaultSurfaceConfig);

            Info_Track currentTrack = new Info_Track_Start(trackStart, data.Context);

            while (currentTrack != null)
            {
                root.AddStop(currentTrack.GetStopData());
                var nextTargetName = currentTrack.GetNextTrackName();
                if (!string.IsNullOrEmpty(nextTargetName)
                    && nextTargetName == trackStart.entity.Name)
                {
                    // we found a cycle. let's just break
                    break;
                }

                var nextTarget = currentTrack.GetNextTrackOrDefault();
                if (nextTarget == null)
                {
                    currentTrack = null;
                }
                else
                {
                    currentTrack = new Info_Track(nextTarget.Value, data.Context);
                }
            }
            root.FinishTrack(entity.GetCarCount(), surface);
        }

        private void SetupTrigger_Checkpoint(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Checkpoint entity = new Trigger_Checkpoint(data.Instance, data.Context);
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
            Trigger_Finish entity = new Trigger_Finish(data.Instance, data.Context);

            var checkpoint = data.Instance.gameObject.GetComponent<TriggerFinish>();
            checkpoint.UpdateMesh();
            checkpoint.OnActivated.AddListener(FinishMap);

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

            CurrentCustomMap.HasFinish = true;
        }

        private void SetupTrigger_Kill(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Kill entity = new Trigger_Kill(data.Instance, data.Context);

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

        private void SetupTrigger_Gravity(BSPLoader.EntityCreatedCallbackData data)
        {
            WrappedEntityInstance entity = new WrappedEntityInstance("trigger_gravity", data.Instance, data.Context);

            var gravityZone = data.Instance.gameObject.GetComponent<AbilityZone>();

            var renderStyle = entity.GetRenderStyle();
            if (renderStyle == WrappedEntityInstance.RenderStyles.Default)
            {
                gravityZone.GetComponent<Renderer>().material = gravityZone.DefaultMaterial;
            }
            else if (renderStyle == WrappedEntityInstance.RenderStyles.Invisible)
            {
                gravityZone.GetComponent<Renderer>().enabled = false;
            }
        }

        private void SetupTrigger_Flight(BSPLoader.EntityCreatedCallbackData data)
        {
            WrappedEntityInstance entity = new WrappedEntityInstance("trigger_flight", data.Instance, data.Context);

            var trigger = data.Instance.gameObject.GetComponent<AbilityTrigger>();

            var renderStyle = entity.GetRenderStyle();
            if (renderStyle == WrappedEntityInstance.RenderStyles.Default)
            {
                trigger.GetComponent<Renderer>().material = trigger.DefaultMaterial;
            }
            else if (renderStyle == WrappedEntityInstance.RenderStyles.Invisible)
            {
                trigger.GetComponent<Renderer>().enabled = false;
            }
        }

        private void SetupTrigger_Teleport(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Teleport entity = new Trigger_Teleport(data.Instance, data.Context);

            var target = entity.GetSingleTarget();

            var teleport = data.Instance.gameObject.GetComponent<Gimmicks.TeleportTrigger>();
            teleport.RespawnPoint = target.gameObject.transform;
            teleport.PreservePhysics = entity.GetPreservePhysics();
            teleport.PhysicsReorientAngle = entity.GetPreservePhysicsAngle();
            teleport.UpdateMesh();

            var renderStyle = entity.GetRenderStyle();

            if (renderStyle == Trigger_Teleport.RenderStyles.Default)
            {
                teleport.GetComponent<Renderer>().material = teleport.DefaultMaterial;
            }
            else if (renderStyle == Trigger_Teleport.RenderStyles.Invisible)
            {
                teleport.GetComponent<Renderer>().enabled = false;
            }
        }

        #endregion

        #region Errors
        private void TextureSource_OnTextureNotFound(object sender, TextureNotFoundEventArgs e)
        {
            CurrentCustomMap.AddError(new Errors.TextureLoadWarning(null, e.TextureName));
        }

        private void ReturnToMainMenuAndShowErrors()
        {
            CustomMap currentMap = CurrentCustomMap;
            gameManager.LoadControlSceneAsync(gameManager.MainMenuControlScene, () =>
            {
                gameManager.DoMainMenuAction((mainMenu) =>
                {
                    mainMenu.OpenPopup(new MainMenu.MenuPopupData()
                    {
                        Title = $"Failed to load {currentMap.Header.MapName}",
                        Body = $"Encountered {currentMap.ErrorCount} errors & {currentMap.WarningCount} warnings.",
                        CancelText = "Close",
                        OkText = "Open Logs",
                        OkPressedCallback= () => OpenMapLoadLogs(PogoGameManager.PogoInstance)
                    });
                });
            });
        }


        private static void WriteMapLoadLogs(GameManager gameManager, CustomMap currentMap)
        {
            Directory.CreateDirectory(gameManager.PersistentDataPath);
            using FileStream file = File.Open($"{gameManager.PersistentDataPath}{Path.DirectorySeparatorChar}CustomMapLoad.log", FileMode.Create);
            using StreamWriter sw = new StreamWriter(file);

            sw.WriteLine($"LOAD map {currentMap.Header.MapName} encountered {currentMap.ErrorCount} Errors & {currentMap.WarningCount} Warnings");
            sw.WriteLine("Earlier errors may lead to later errors, so you should try to solve from the top down.");
            sw.WriteLine("Running into trouble? Get help online in the steam community forums or the official discord");
            sw.WriteLine();

            int index = 1;
            foreach (var error in currentMap.Errors)
            {
                string type = error.Severity switch
                {
                    Errors.MapError.Severities.Error =>   "ERROR",
                    Errors.MapError.Severities.Warning => "WARNING",
                    _ => "????",
                };

                sw.WriteLine($"#### {type} {index++} ".PadRight(80, '#'));
                sw.Write(error.ToLogString());
                sw.WriteLine();
                sw.WriteLine();
            }
        }

        private static void OpenMapLoadLogs(GameManager gameManager)
        {
            Application.OpenURL($"file:///{gameManager.PersistentDataPath}{Path.DirectorySeparatorChar}CustomMapLoad.log");
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
