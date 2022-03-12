using Pogo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WizardUtils;

[CustomEditor(typeof(PogoGameManager))]
public class PogoGameManagerEditor : GameManagerEditor
{
    new PogoGameManager self => base.self as PogoGameManager;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorGUILayout.DropdownButton(new GUIContent("Change Level to..."), FocusType.Passive))
        {
            GenericMenu menu = new GenericMenu();

            var levels = AssetDatabase.FindAssets($"t:{nameof(LevelDescriptor)}")
                .Select(id => AssetDatabase.LoadAssetAtPath<LevelDescriptor>(AssetDatabase.GUIDToAssetPath(id)));
            foreach(var level in levels)
            {
                menu.AddItem(new GUIContent(level.name), level == self.InitialLevel, () =>
                {
                    self.InitialLevel = level;
                    if (self.InControlScene) self.UnloadControlSceneInEditor();
                    self.GetComponent<PogoLevelManager>().LoadLevelInEditor(level);
                    self.CurrentControlScene = null;
                });
            }
            menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
        }

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

                    self.RespawnPoint.position = spawnPoint.transform.position;
                    self.RespawnPoint.rotation = Quaternion.Euler(0, spawnPoint.transform.rotation.eulerAngles.y, 0);
                    Undo.RecordObject(self.RespawnPoint, "Move Player to Spawnpoint");

                    var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
                    foreach (PlayerController player in results)
                    {
                        player.transform.position = self.RespawnPoint.position;
                        player.transform.rotation = Quaternion.Euler(0, self.RespawnPoint.rotation.eulerAngles.y, 0);
                        Undo.RecordObject(player, "Move Player to Spawnpoint");
                    }

                    Selection.activeObject = self.RespawnPoint;
                    Undo.CollapseUndoOperations(undoGroup);
                });
            }
            menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
        }

        if (GUILayout.Button("Move Player to Spawnpoint"))
        {
            var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
            foreach (PlayerController player in results)
            {
                player.transform.position = self.RespawnPoint.position;
                player.transform.rotation = Quaternion.Euler(0, self.RespawnPoint.rotation.eulerAngles.y, 0);
                Undo.RecordObject(player, "Move Player to Spawnpoint");
            }
        }
    }
}
