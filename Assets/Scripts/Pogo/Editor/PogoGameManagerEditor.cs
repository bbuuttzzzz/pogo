using Pogo;
using Pogo.Levels;
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

    public override void OnControlSceneLoadedInEditor()
    {
        self.GetComponent<PogoLevelManager>().CurrentLevel = null;
        self.InitialLevel = null;
        base.OnControlSceneLoadedInEditor();
    }

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
                if (level.HideInEditor) continue;

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

            var itempaths = new List<(GameObject item, string path)>();
            foreach(var spawnPoint in spawnPoints)
            {
                string path = AnimationUtility.CalculateTransformPath(spawnPoint.transform, null);
                path = path.Replace("/", " -> ");

                itempaths.Add((spawnPoint, path));
            }

            GenericMenu menu = new GenericMenu();
            foreach (var itempath in itempaths.OrderBy(ip => ip.path))
            {
                menu.AddItem(new GUIContent(itempath.path), false, () =>
                {
                    Undo.SetCurrentGroupName("Move Spawnpoint");
                    int undoGroup = Undo.GetCurrentGroup();

                    ExplicitCheckpoint checkpointTrigger = itempath.item.GetComponentInParent<ExplicitCheckpoint>();
                    if (checkpointTrigger != null)
                    {
                        self._CachedCheckpoint = checkpointTrigger.Descriptor;
                    }
                    else
                    {
                        self._CachedCheckpoint = null;
                    }

                    SetSpawnPointInEditor(self, itempath.item.transform);

                    Undo.CollapseUndoOperations(undoGroup);
                });
            }
            menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
        }

        using(new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Select Point"))
            {
                var previousSelection = Selection.activeObject;
                Selection.activeObject = self.CachedRespawnPoint.transform;
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                    Selection.activeObject = previousSelection;
                };
            }

            if (GUILayout.Button("Move Player"))
            {
                MovePlayerToSpawnPoint();
            }
        }
        
    }

    public static void SetSpawnPointInEditor(PogoGameManager self, Transform newSpawnPoint)
    {
        self.CachedRespawnPoint.transform.position = newSpawnPoint.position;
        self.CachedRespawnPoint.transform.rotation = Quaternion.Euler(0, newSpawnPoint.rotation.eulerAngles.y, 0);
        Undo.RecordObject(self.CachedRespawnPoint.transform, "Move Player to Spawnpoint");
        MovePlayerToSpawnPoint(newSpawnPoint);
    }

    private void MovePlayerToSpawnPoint() => MovePlayerToSpawnPoint(self.CachedRespawnPoint);

    public static void MovePlayerToSpawnPoint(Transform spawnPoint)
    {
        var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
        foreach (PlayerController player in results)
        {
            using (new UndoScope("Move Player to Spawn Point"))
            {
                Undo.RecordObject(player, "Move Player to Spawnpoint");
                Undo.RecordObject(player.CollisionGroup.transform, "");
                Undo.RecordObject(player.RenderTransform, "");
                Undo.RecordObject(player.RenderPivotTransform, "");
                player.TeleportToInEditor(spawnPoint);

                var previousSelection = Selection.activeObject;
                Selection.activeObject = spawnPoint.transform;
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
                    Selection.activeObject = previousSelection;
                };
            }
        }
    }
}
