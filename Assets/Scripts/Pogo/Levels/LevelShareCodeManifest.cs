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

        [Tooltip("Lower Number = Displayed First")]
        public int EditorDisplayPriority;

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