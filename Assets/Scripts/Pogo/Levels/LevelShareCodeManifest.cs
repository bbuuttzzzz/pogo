using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pogo.Levels
{
    [CreateAssetMenu(fileName = "levels_", menuName = "Pogo/LevelManifest", order = 1)]
    public class LevelShareCodeManifest : ScriptableObject
    {
        [HideInInspector]
        public ShareCode[] ShareCodes;

        [NonSerialized]
        public LevelDescriptor[] Levels;

        private void Awake()
        {
            Levels = ShareCodes
                .Select(s => s.LevelState.Level)
                .Distinct()
                .ToArray();
        }

        [Tooltip("Lower Number = Displayed First")]
        public int EditorDisplayPriority;

        /// <summary>
        /// Find the ShareCode of the provided <paramref name="levelState"/><br/>
        /// If <paramref name="enableSoftMatching"/> is true (default), return the result for an equivalent state
        /// </summary>
        /// <param name="levelState"></param>
        /// <param name="manifest"></param>
        /// <param name="result"></param>
        /// <param name="enableSoftMatching">if true, if there's no exact match, search in the negative direction for the next shared state</param>
        /// <returns></returns>
        public bool TryGetShareCode(LevelState levelState, out ShareCode result)
        {
            int bestStateId = -1;

            for (int i = 0; i < ShareCodes.Length; i++)
            {
                ShareCode shareCode = ShareCodes[i];
                if (shareCode.LevelState == levelState)
                {
                    result = shareCode;
                    return true;
                }
                else if (bestStateId == -1
                    && shareCode.LevelState.StateId < levelState.StateId)
                {
                    bestStateId = i;
                }
                else if (bestStateId != -1
                    && shareCode.LevelState.StateId < ShareCodes[bestStateId].LevelState.StateId)
                {
                    bestStateId = i;
                }
            }

            if (bestStateId >= 0)
            {
                result = ShareCodes[bestStateId];
                return true;
            }

            result = default;
            return false;
        }

        public bool TryGetShareCodeExactly(LevelState levelState, out ShareCode result)
        {
            for (int i = 0; i < ShareCodes.Length; i++)
            {
                ShareCode shareCode = ShareCodes[i];
                if (shareCode.LevelState == levelState)
                {
                    result = shareCode;
                    return true;
                }
            }

            Debug.LogError(($"levelState \'{levelState}\' was not valid for Manifest \'{this}\'"));
            result = default;
            return false;
        }

        public bool ShareIndexExists(int shareIndex) => TryGetLevelState(shareIndex, out _);
        public bool TryGetLevelState(int shareIndex, out LevelState result)
        {
            foreach(var ShareCode in ShareCodes)
            {
                if (ShareCode.ShareIndex == shareIndex)
                {
                    result = ShareCode.LevelState;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public int GetNextUnusedShareIndex()
        {
            int index = 1;
            foreach(var code in ShareCodes.OrderBy(x => x.ShareIndex))
            {
                if (index < code.ShareIndex)
                {
                    return index;
                }
                else
                {
                    index = code.ShareIndex + 1;
                }
            }

            if (index < 256)
            {
                return index;
            }
            throw new KeyNotFoundException("Couldn't find any unused ShareIndexes :(");
        }

        public LevelShareCodeGroup GetCodeGroupForLevel(LevelDescriptor level)
        {
            var codes = ShareCodes
                .Where(c => c.LevelState.Level == level)
                .ToList();

            LevelShareCodeGroup group = new LevelShareCodeGroup()
            {
                Level = level,
                Codes = codes
            };

            return group;
        }

        public LevelShareCodeGroup[] CollateCodesByLevel()
        {
            return ShareCodes
                .GroupBy(sc => sc.LevelState.Level)
                .Select(g => new LevelShareCodeGroup()
                {
                    Level = g.Key,
                    Codes = g.ToList()
                })
                .ToArray();
        }

        public void SetFromCollation(LevelShareCodeGroup[] groups)
        {
            List<ShareCode> codes = new List<ShareCode>();
            foreach(var group in groups)
            {
                codes.AddRange(group.Codes);
            }

            ShareCodes = codes.ToArray();
        }

        public void UpdateWithGroup(LevelShareCodeGroup group)
        {
            ShareCodes = ShareCodes
                .Where(sc => sc.LevelState.Level != group.Level)
                .Concat(group.Codes)
                .ToArray();
        }
    }
}