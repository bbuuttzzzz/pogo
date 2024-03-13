using Pogo.CustomMaps.Materials;
using UnityEngine;

namespace Pogo.CustomMaps
{
    [CreateAssetMenu(fileName = "skymat_", menuName = "Pogo/CustomMaps/SkyMaterialDescriptor")]
    public class SkyMaterialDescriptor : ScriptableObject
    {
        public string Key;
        public FillableShaderDescriptor FillableShader;

        public string TextureName;
        [HideInInspector]
        public string[] PropertyValues;
    }
}
