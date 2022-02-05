using Pogo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PogoGameManager))]
public class PogoGameManagerEditor : Editor
{
    PogoGameManager self;

    public override VisualElement CreateInspectorGUI()
    {
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        self = target as PogoGameManager;

        if (GUILayout.Button("Move Player to Waypoint"))
        {
            var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
            foreach (PlayerController player in results)
            {
                player.transform.position = self.RespawnPointTransform.position;
                player.transform.rotation = Quaternion.Euler(0, self.RespawnPointTransform.rotation.eulerAngles.y, 0);
                Undo.RecordObject(player, "Move Player to Waypoint");
            }
        }

        var newLevel = EditorGUILayout.ObjectField(new GUIContent("Current Level"), self.InitialLevel, typeof(LevelDescriptor), false) as LevelDescriptor;
        if (newLevel != self.InitialLevel)
        {
            self.InitialLevel = newLevel;
            PogoLevelManager.LoadLevelInEditor(self.InitialLevel);
        }
    }
}
