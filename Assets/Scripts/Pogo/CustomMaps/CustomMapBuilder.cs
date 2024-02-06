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
        }

        private void OnEntityCreated(BSPLoader.EntityInstance instance, List<BSPLoader.EntityInstance> targetList)
        {
            if (EntityHandlers.TryGetValue(instance.entity.ClassName, out var handler))
            {
                handler.SetupAction.Invoke(instance, targetList);
            }
        }

        #region Entity Handlers
        private void SetupEntityHandlers()
        {
            EntityHandlers = new Dictionary<string, CustomMapEntityHandler>();

            // we dont need to handle info_player_respawn. it's handled by trigger_checkpoint
            AddEntityHandler(new CustomMapEntityHandler("trigger_checkpoint", SetupTrigger_Checkpoint));
        }

        private void AddEntityHandler(CustomMapEntityHandler handler) => EntityHandlers.Add(handler.ClassName, handler);

        private void SetupTrigger_Checkpoint(BSPLoader.EntityInstance instance, List<BSPLoader.EntityInstance> list)
        {
            Trigger_Checkpoint entity = new Trigger_Checkpoint(instance);
            CheckpointId id = entity.GetCheckpointId();
            
            if (list.Count == 0)
            {
                throw new FormatException($"{instance.entity.ClassName} with ID {id} has no defined Target. are you missing an info_player_respawn?");
            }

            var collider = instance.gameObject.AddComponent<MeshCollider>();
            collider.convex = true;

            var checkpoint = instance.gameObject.AddComponent<GeneratedCheckpoint>();
            checkpoint.FixTriggerSettings();
            checkpoint.Id = id;
            checkpoint.RespawnPoint = list[0].gameObject.transform;
            checkpoint.CanSkip = entity.GetCanSkip();
            if (checkpoint.CanSkip && !string.IsNullOrEmpty(entity.GetOverrideSkipTarget()))
            {
                throw new NotImplementedException();
            }
            CurrentCustomMap.RegisterCheckpoint(checkpoint);
        }
        #endregion
    }
}
