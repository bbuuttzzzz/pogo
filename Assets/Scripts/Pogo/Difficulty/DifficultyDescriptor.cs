using Assets.Scripts.Pogo.Difficulty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    [CreateAssetMenu(fileName = "diff_", menuName = "Pogo/DifficultyDescriptor")]
    public class DifficultyDescriptor : ScriptableObject
    {
        public Difficulties DifficultyEnum;
        public string DisplayName;
        public Mesh SkullMesh;
        public Material SkullMaterial;
        [TextArea(4,10)]
        public string Description;
    }
}
