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

namespace Pogo.CustomMaps
{
    public class CustomMapBuilder : MonoBehaviour
    {
        public LevelDescriptor CustomMapLevel;
        public GameObject CheckpointPrefab;
        public string TexturePath;
        public CustomMap CurrentCustomMap;
        public Dictionary<string, CustomMapEntityHandler> EntityHandlers { get; private set; }

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
                meshCombineOptions = BSPLoader.MeshCombineOptions.PerEntity,
                assetSavingOptions = BSPLoader.AssetSavingOptions.None,
                curveTessellationLevel = 3,
                entityCreatedCallback = OnEntityCreated,
                scaleFactor = 1f / 32f
            };
            var textureSource = new WadFolderSource(folderPath);
            var loader = new BSPLoader(settings, textureSource);
            loader.LoadBSP();

            foreach(var checkpoint in CurrentCustomMap.Checkpoints.Values)
            {
                FinishSettingUpTrigger_Checkpoint(checkpoint);
            }
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

            var collider = data.Instance.gameObject.AddComponent<MeshCollider>();
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
