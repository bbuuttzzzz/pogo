using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Pogo.Levels
{
    [CreateAssetMenu(fileName = "new level", menuName = "Pogo/LevelDescriptor", order = 1)]
    public class LevelDescriptor : ScriptableObject
    {
        [Tooltip("These other levels should be visible while the player is on this level")]
        public LevelDescriptor[] AdjacentLevels;

        public GameObject PostProcessingPrefab;

        /// <summary>
        /// all levels that should be loaded. this includes itself
        /// </summary>
        public List<LevelDescriptor> LoadLevels
        {
            get
            {
                var list = new List<LevelDescriptor>(AdjacentLevels.Length + 1);
                list.AddRange(AdjacentLevels);
                list.Add(this);
                return list;
            }
        }

        public int BuildIndex;

        [Range(0, 255)]
        public int ShareIndex;

        public string ScenePath => SceneUtility.GetScenePathByBuildIndex(BuildIndex);

        public bool HideInEditor = false;
        public int LevelStatesCount;
    }
}