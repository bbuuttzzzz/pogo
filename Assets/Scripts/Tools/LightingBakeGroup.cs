using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pogo.Tools
{
    [CreateAssetMenu(fileName ="bakeGroup_", menuName = "Pogo/Lighting Bake Group")]
    public class LightingBakeGroup : ScriptableObject
    {
        public SceneAsset MainScene;
        public SceneAsset[] OtherScenes;
    }
}
