using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardEffects;
using WizardUtils.SerializedObjectHelpers;

namespace Pogo
{
    [CustomEditor(typeof(FlagConfigurer))]
    public class FlagConfigurerEditor : Editor
    {
        private FlagConfigurer self;

        private SerializedProperty m_Radius;
        private SerializedProperty m_FlagOffset;

        public override VisualElement CreateInspectorGUI()
        {
            self = (FlagConfigurer)target;

            m_Radius = serializedObject.FindProperty(nameof(self.Radius));
            m_FlagOffset = serializedObject.FindProperty(nameof(self.FlagOffset));

            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(m_Radius);
            EditorGUILayout.PropertyField(m_FlagOffset);
            bool oldHardModeEnabled = GetHardModeEnabled();
            bool newHardModeEnabled = EditorGUILayout.Toggle("Hard Mode Enabled", oldHardModeEnabled);

            if (oldHardModeEnabled != newHardModeEnabled)
            {
                SetHardModeEnabled(newHardModeEnabled);
            }

            var updater = new SerializedObjectUpdater(serializedObject);

            updater.Add(() => self.Radius, OnLineShapeChanged);
            updater.Add(() => self.FlagOffset, OnLineShapeChanged);

            updater.ApplyModifiedProperties();
        }
        private bool GetHardModeEnabled()
        {
            RespawnPoint respawnPoint = self.transform.parent.GetComponentInChildren<RespawnPoint>();
            if (respawnPoint == null)
            {
                Debug.LogWarning($"Missing RespawnPoint by FlagConfigurer", self);
                return false;
            }

            return respawnPoint.EnabledInHardMode;
        }

        private void SetHardModeEnabled(bool value)
        {
            using var scope = new UndoScope("Set Hard Mode Enabled");

            RespawnPoint respawnPoint = self.transform.parent.GetComponentInChildren<RespawnPoint>();
            if (respawnPoint == null)
            {
                Debug.LogWarning($"Missing RespawnPoint by FlagConfigurer", self);
                return;
            }

            Undo.RecordObject(respawnPoint, "Set Hard Mode Enabled");
            respawnPoint.EnabledInHardMode = value;
            EditorUtility.SetDirty(respawnPoint);

            var material = value ? self.FlagMaterial_Star : self.FlagMaterial_NoStar;
            if (self.Flag1 != null)
            {
                Undo.RecordObject(self.Flag1, "");
                self.Flag1.GetComponent<Renderer>().sharedMaterial = material;
                EditorUtility.SetDirty(self.Flag1);
            }
            if (self.Flag2 != null)
            {
                Undo.RecordObject(self.Flag2, "");
                self.Flag2.GetComponent<Renderer>().sharedMaterial = material;
                EditorUtility.SetDirty(self.Flag2);
            }
        }

        private void OnLineShapeChanged(SerializedPropertyChangedArgs<float> args)
        {
            using var scope = new UndoScope("change line shape");


            if (self.Flag1 != null)
            {
                Undo.RecordObject(self.Flag1, "");
                self.Flag1.localPosition = Vector3.left * (self.Radius - self.FlagOffset);
            }

            if (self.Flag2 != null)
            {
                Undo.RecordObject(self.Flag2, "");
                self.Flag2.localPosition = Vector3.right * (self.Radius - self.FlagOffset);
            }

            foreach (var particleLine in self.ParticleLines)
            {
                if (particleLine != null)
                {
                    Undo.RecordObject(particleLine, "");
                    particleLine.UpdateEffect();
                }
            }
            if (self.Collider != null)
            {
                Undo.RecordObject(self.Collider, "");
                Vector3 size = self.Collider.size;
                size.x = self.Radius * 2;
                self.Collider.size = size;
            }
        }
    }
}