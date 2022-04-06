using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Extensions
{
    public static class GizmosHelper
    {
        public static void DrawCubeFlush(Vector3 point, Vector3 up, Vector3 size)
        {
            Vector3 center = point + up * size.y / 2;
            Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.right, up), up);
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, size);
            Gizmos.matrix = Matrix4x4.identity;
        }


        public static void DrawWireMeshFlush(Mesh mesh, Vector3 point, Vector3 up, Quaternion rotation)
        {
            DrawWireMeshFlush(mesh, point, up, rotation, Vector3.one);
        }
        public static void DrawWireMeshFlush(Mesh mesh, Vector3 point, Vector3 up, Quaternion rotation, Vector3 scale)
        {
            Quaternion finalRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.right, up), up) * rotation;
            Gizmos.DrawWireMesh(mesh, point, finalRotation, scale);
        }

        public static void DrawMeshFlush(Mesh mesh, Vector3 point, Vector3 up, Quaternion rotation)
        {
            DrawMeshFlush(mesh, point, up, rotation, Vector3.one);
        }
        public static void DrawMeshFlush(Mesh mesh, Vector3 point, Vector3 up, Quaternion rotation, Vector3 scale)
        {
            Quaternion finalRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.right, up), up) * rotation;
            Gizmos.DrawWireMesh(mesh, point, finalRotation, scale);
        }
    }
}
