using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pogo.Levels
{
    [CreateAssetMenu(fileName = "levels_", menuName = "Pogo/LevelManifest", order = 1)]
    public class LevelShareCodeManifest : ScriptableObject
    {
        [HideInInspector]
        public ShareCode[] ShareCodes;

        [Tooltip("Lower Number = Displayed First")]
        public int EditorDisplayPriority;

        public LevelShareCodeGroup GetCodesForLevel(LevelDescriptor level)
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
            List<ShareCode> existingCodes = ShareCodes
                .Where(sc => sc.LevelState.Level != group.Level)
                .ToList();

            existingCodes.AddRange(group.Codes);
        }
    }
}