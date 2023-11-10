using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;

namespace PoseClipboard
{
    [CustomEditor(typeof(TrailPoseClipboard))]
    public class TrailPoseClipboardEditor : Editor
    {
        private TrailPoseClipboard self;
        public override VisualElement CreateInspectorGUI()
        {
            self = target as TrailPoseClipboard;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Copy Pose"))
            {
                Copy();
            }

            if (GUILayout.Button("Paste Pose"))
            {
                Paste();
            }

            base.OnInspectorGUI();
        }

        public void Copy()
        {
            GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(GetData(), new JsonSerializerSettings()
            {
                Converters = new[]
                {
                    new FlatVector3Converter()
                }
            });
        }

        public void Paste()
        {
            TrailData data = JsonConvert.DeserializeObject<TrailData>(GUIUtility.systemCopyBuffer, new JsonSerializerSettings()
            {
                Converters = new[]
                {
                    new FlatVector3Converter()
                }
            });
            SetData(data);
        }

        public TrailData GetData()
        {
            Vector3[] positions = new Vector3[self.Target.positionCount];
            self.Target.GetPositions(positions);

            return new TrailData()
            {
                Positions = positions,
            };
        }

        public void SetData(TrailData data)
        {
            Undo.RecordObject(self.Target, "Set Trail Pose");

            self.Target.Clear();
            self.Target.AddPositions(data.Positions);
        }


        [System.Serializable]
        public struct TrailData
        {
            public Vector3[] Positions;
        }
    }
}
