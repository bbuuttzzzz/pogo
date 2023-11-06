using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using WizardUtils.Math;

namespace Pogo.Levels
{
    [CreateAssetMenu(fileName = "new level", menuName = "Pogo/LevelDescriptor", order = 1)]
    public class LevelDescriptor : ScriptableObject
    {
        [Tooltip("These other levels should be visible while the player is on this level")]
        public LevelDescriptor[] AdjacentLevels;

        [Tooltip("Loading into this level will set these states, but not override existing states. If you don't supply one for THIS level, defaults to zero")]
        public LevelState[] InitialLevelStates;

        public Vector3 ShareOrigin;

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

        /// <summary>
        /// all InitialLevelStates that should be loaded. this includes (this, 0) if this isn't included already
        /// </summary>
        public IEnumerable<LevelState> LoadLevelStates
        {
            get
            {
                if (InitialLevelStates.Any(ls => ls.Level == this))
                {
                    return InitialLevelStates;
                }
                else
                {
                    return InitialLevelStates
                        .Append(new LevelState(this, 0));
                }
            }
        }

        public int BuildIndex;

#if UNITY_EDITOR
        [Multiline(8)]
        public string Notes;
#endif

        public string ScenePath => SceneUtility.GetScenePathByBuildIndex(BuildIndex);

        public bool HideInEditor = false;
        public int LevelStatesCount;
    }
}