using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    [CreateAssetMenu(fileName = "cmat_", menuName = "Pogo/CustomMaps/FillableShaderDescriptor")]
    public class FillableShaderDescriptor : ScriptableObject, IFillableShader
    {
        public string ShaderName;
        public Material BaseMaterial;
        public FillableShaderProperty[] Properties;

        string IFillableShader.ShaderName => ShaderName;

        Material IFillableShader.BaseMaterial => BaseMaterial;

        FillableShaderProperty[] IFillableShader.Properties => Properties;
    }
}
