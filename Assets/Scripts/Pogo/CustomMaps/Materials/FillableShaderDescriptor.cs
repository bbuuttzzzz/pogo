using Assets.Scripts.Pogo.CustomMaps.Materials;
using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    [CreateAssetMenu(fileName = "cmat_", menuName = "Pogo/CustomMaps/FillableShaderDescriptor")]
    public class FillableShaderDescriptor : ScriptableObject
    {
        public Material BaseMaterial;
        public FillableShaderProperty[] Properties;
    }
}
