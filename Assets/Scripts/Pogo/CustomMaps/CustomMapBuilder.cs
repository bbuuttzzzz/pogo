using BSPImporter;
using BSPImporter.Textures;
using Pogo.Checkpoints;
using Pogo.CustomMaps.Entities;
using Pogo.Difficulties;
using Pogo.Levels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Pogo.CustomMaps
{
    public class CustomMapBuilder : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public LevelDescriptor CustomMapLevel;
        public CustomMap CurrentCustomMap;
        public Material DefaultMaterial;
        public Material DefaultCheckpointMaterial;
        
        public Dictionary<string, CustomMapEntityHandler> EntityHandlers { get; private set; }

        private void Start()
        {
            gameManager = PogoGameManager.PogoInstance;
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
            var textureSource = new WadFolderSource(folderPath);
            var loader = new BSPLoader(settings, textureSource);
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

            foreach (var checkpoint in CurrentCustomMap.Checkpoints.Values)
            {
                FinishSettingUpTrigger_Checkpoint(checkpoint);
            }

            gameManager.RegisterRespawnPoint(new RespawnPointData(CurrentCustomMap.FirstCheckpoint.RespawnPoint));
            PogoGameManager.PogoInstance.SpawnPlayer();                  
            gameManager.Paused = false;
        }

        private void OnEntityCreated(BSPLoader.EntityCreatedCallbackData data)
        {
            if (EntityHandlers.TryGetValue(data.Instance.entity.ClassName, out var handler))
            {
                handler.SetupAction.Invoke(data);
            }
        }

        #region Entity Handlers
        private void SetupEntityHandlers()
        {
            EntityHandlers = new Dictionary<string, CustomMapEntityHandler>();

            // we dont need to handle info_player_respawn. it's handled by trigger_checkpoint
            AddEntityHandler(new CustomMapEntityHandler("trigger_checkpoint", SetupTrigger_Checkpoint));
            AddEntityHandler(new CustomMapEntityHandler("trigger_finish", SetupTrigger_Finish));
        }

        private void AddEntityHandler(CustomMapEntityHandler handler) => EntityHandlers.Add(handler.ClassName, handler);

        private void SetupTrigger_Checkpoint(BSPLoader.EntityCreatedCallbackData data)
        {
            Trigger_Checkpoint entity = new Trigger_Checkpoint(data);
            CheckpointId id = entity.GetCheckpointId();

            var target = entity.GetSingleTarget();

            var collider = data.Instance.gameObject.GetComponent<MeshCollider>();
            collider.convex = true;

            var checkpoint = data.Instance.gameObject.AddComponent<GeneratedCheckpoint>();
            checkpoint.FixTriggerSettings();
            checkpoint.Id = id;
            checkpoint.RespawnPoint = target.gameObject.transform;
            checkpoint.CanSkip = entity.GetCanSkip();
            if (checkpoint.CanSkip && !string.IsNullOrEmpty(entity.GetOverrideSkipTargetName()))
            {
                var skipTarget = entity.GetSingleOverrideSkipTarget();
                checkpoint.SkipTarget = skipTarget.gameObject.transform;
            }

            var renderStyle = entity.GetRenderStyle();

            if (renderStyle == Trigger_Checkpoint.RenderStyles.Default)
            {
                checkpoint.GetComponent<Renderer>().material = DefaultCheckpointMaterial;
            }
            else if (renderStyle == Trigger_Checkpoint.RenderStyles.Invisible)
            {
                checkpoint.GetComponent<Renderer>().enabled = false;
            }

            try
            {
                CurrentCustomMap.RegisterCheckpoint(checkpoint);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"Map contains duplicate checkpoints with pathtype {id.CheckpointType} & number {id.CheckpointNumber}", e);
            }
        }

        private void SetupTrigger_Finish(BSPLoader.EntityCreatedCallbackData data)
        {
            //throw new NotImplementedException();
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
