using UnityEngine;

namespace WizardUtils
{
    public static class MeshHelper
    {
        public static Material GetMaterialAtTriangle(this Mesh mesh, MeshRenderer renderer, int triangleIndex)
        {
            // get the submesh for this triangle
            var selectedSubmeshIndex = mesh.GetSubmeshIndex(triangleIndex);

            // then we return the material for that submesh
            var materials = renderer.sharedMaterials;
            return materials.Length - 1 < selectedSubmeshIndex ? materials[0] : materials[selectedSubmeshIndex];
        }

        public static int GetSubmeshIndex(this Mesh mesh, int triangleIndex)
        {
            int triangleCounter = 0;
            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                var indexCount = mesh.GetSubMesh(subMeshIndex).indexCount;
                triangleCounter += indexCount / 3;
                if (triangleIndex < triangleCounter)
                {
                    return subMeshIndex;
                }
            }

            return 0;
        }
    }
}
