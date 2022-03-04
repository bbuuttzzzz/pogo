using UnityEditor;
using UnityEngine;

namespace WizardEffects.Inspector
{
    [CustomEditor(typeof(ParticleLine))]
    class ParticleLineEditor : Editor
    {
        ParticleLine self;

        public override void OnInspectorGUI()
        {
            self = target as ParticleLine;
            DrawDefaultInspector();

            if (GUILayout.Button("Update Effect"))
            {
                self.UpdateEffect();
                Undo.RecordObject(self, "Update Effect");
            }
        }
    }
}