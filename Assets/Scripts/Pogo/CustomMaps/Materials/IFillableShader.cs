using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps.Materials
{
    public interface IFillableShader
    {
        public string ShaderName { get; }
        public Material BaseMaterial { get; }
        public FillableShaderProperty[] Properties { get; }
        public bool ReplaceMainTex { get; }
    }
}
