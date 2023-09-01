using Assets.Scripts.Pogo.Difficulty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu(fileName = "DifficultyManifest", menuName = "Pogo/DifficultyManifest")]
    public class DifficultyManifest : ScriptableObject
    {
        public DifficultyDescriptor[] Difficulties;

        public DifficultyDescriptor FindByKey(DifficultyId difficulty)
        {
            foreach(var difficultyDescriptor in Difficulties)
            {
                if (difficultyDescriptor.DifficultyEnum == difficulty)
                {
                    return difficultyDescriptor;
                }
            }

            throw new KeyNotFoundException();
        }
    }
}
