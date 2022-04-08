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
        public static void DrawSphereConnection(Vector3 center1, Vector3 center2, float radius1, float radius2)
        {
            Quaternion along = Quaternion.LookRotation(center2 - center1, Vector3.up);

            for(int n = 0; n < 4; n++)
            {
                Quaternion around = Quaternion.Euler(0, 90 * n, 0);
                Vector3 start = center1 + around * along * Vector3.right * radius1;
                Vector3 end = center2 + around * along * Vector3.right * radius2;

                Gizmos.DrawLine(start, end);
            }
        }

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
