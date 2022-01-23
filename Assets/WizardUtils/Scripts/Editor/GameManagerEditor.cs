using Pogo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PogoGameManager))]
public class PogoGameManagerEditor : Editor
{
    PogoGameManager self;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Move Player to Waypoint"))
        {
            self = target as PogoGameManager;
            var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
            if (results[0] != null)
            {
                var player = results[0] as PlayerController;

                player.transform.position = self.RespawnPoint.position;
                player.transform.rotation = Quaternion.Euler(0, self.RespawnPoint.rotation.eulerAngles.y, 0);
                Undo.RecordObject(player, "Move Player to Waypoint");
            }
        }    
    }
}
