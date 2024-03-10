using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    [System.Serializable]
    public struct FillableShaderProperty
    {
        public enum ParameterTypes
        {
            Float,
            Color,
            Vector2,
            Vector3
        }

        [Tooltip("The name of the property in the wad definition")]
        public string PropertyName;
        [Tooltip("The name of the parameter in the shader")]
        public string ParameterName;
        [Tooltip("The type of the specified parameter")]
        public ParameterTypes ParameterType;
    }
}
