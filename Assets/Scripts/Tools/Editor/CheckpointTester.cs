using NUnit.Framework.Internal;
using Pogo;
using Pogo.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tools.Editor
{
    public static class CheckpointTester
    {

        //[MenuItem("Pogo/Check Checkpoints")]
        public static void CheckCheckpoints()
        {
            string eventName = nameof(ExplicitCheckpoint.OnSkip);
            // Get the field information for the event using reflection.
            FieldInfo eventField = typeof(ExplicitCheckpoint).GetField(eventName, BindingFlags.Instance | BindingFlags.Public); FieldInfo callsfield = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
            int count = 0;
            MultiSceneComponentEnumerator<ExplicitCheckpoint> enumerator = new MultiSceneComponentEnumerator<ExplicitCheckpoint>();
            foreach (var checkpoint in enumerator)
            {
                UnityEventBase unityEvent = (UnityEventBase)eventField.GetValue(checkpoint);
                object calls = callsfield.GetValue(unityEvent);
                // Try to get the Count property using reflection.
                PropertyInfo countProperty = calls.GetType().GetProperty("Count");
                int callCount = (int)countProperty.GetValue(calls);
                if (callCount > 0)
                {
                    count++;
                    Debug.LogWarning($"ExplicitCheckpoint {checkpoint.Descriptor} in scene {enumerator.CurrentScenePath} has {callCount} calls on {eventName}");
                }
            }
            Debug.LogWarning($"Finished! found {count} offending checkpoint Triggers");
        }
    }
}
