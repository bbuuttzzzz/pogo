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

        var newLevel = EditorGUILayout.ObjectField(new GUIContent("Current Level"), self.InitialLevel, typeof(LevelDescriptor), false) as LevelDescriptor;
        if (newLevel != self.InitialLevel)
        {
            self.InitialLevel = newLevel;
            PogoLevelManager.LoadLevelInEditor(self.InitialLevel);
        }

        GUILayout.BeginHorizontal();
        if (EditorGUILayout.DropdownButton(new GUIContent("Move Spawnpoint to..."), FocusType.Passive))
        {
            var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            GenericMenu menu = new GenericMenu();
            foreach(var spawnPoint in spawnPoints)
            {
                string path = AnimationUtility.CalculateTransformPath(spawnPoint.transform, null);
                path = path.Replace("/"," -> ");
                menu.AddItem(new GUIContent(path), false, () =>
                {
                    Undo.SetCurrentGroupName("Move Spawnpoint");
                    int undoGroup = Undo.GetCurrentGroup();

                    self.InitialRespawnPoint.position = spawnPoint.transform.position;
                    self.InitialRespawnPoint.rotation = Quaternion.Euler(0, spawnPoint.transform.rotation.eulerAngles.y, 0);
                    Undo.RecordObject(self.InitialRespawnPoint, "Move Player to Spawnpoint");

                    var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
                    foreach (PlayerController player in results)
                    {
                        player.transform.position = self.InitialRespawnPoint.position;
                        player.transform.rotation = Quaternion.Euler(0, self.InitialRespawnPoint.rotation.eulerAngles.y, 0);
                        Undo.RecordObject(player, "Move Player to Spawnpoint");
                    }

                    Undo.CollapseUndoOperations(undoGroup);
                });
            }
            menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Move Player to Spawnpoint"))
        {
            var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
            foreach (PlayerController player in results)
            {
                player.transform.position = self.InitialRespawnPoint.position;
                player.transform.rotation = Quaternion.Euler(0, self.InitialRespawnPoint.rotation.eulerAngles.y, 0);
                Undo.RecordObject(player, "Move Player to Spawnpoint");
            }
        }
    }
}
