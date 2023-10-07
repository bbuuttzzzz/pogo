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

                    self.CachedRespawnPoint.transform.position = itempath.item.transform.position;
                    self.CachedRespawnPoint.transform.rotation = Quaternion.Euler(0, itempath.item.transform.rotation.eulerAngles.y, 0);
                    Undo.RecordObject(self.CachedRespawnPoint.transform, "Move Player to Spawnpoint");

                    var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
                    foreach (PlayerController player in results)
                    {
                        player.PhysicsPosition= self.CachedRespawnPoint.transform.position;
                        player.PhysicsRotation= Quaternion.Euler(0, self.CachedRespawnPoint.transform.rotation.eulerAngles.y, 0);
                        Undo.RecordObject(player, "Move Player to Spawnpoint");
                    }

                    Undo.CollapseUndoOperations(undoGroup);
                });
            }
            menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
        }

        using(new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Select Point"))
            {
                Selection.activeObject = self.CachedRespawnPoint.transform;
            }

            if (GUILayout.Button("Move Player"))
            {
                var results = Resources.FindObjectsOfTypeAll(typeof(PlayerController));
                foreach (PlayerController player in results)
                {
                    player.PhysicsPosition = self.CachedRespawnPoint.position;
                    player.PhysicsRotation = Quaternion.Euler(0, self.CachedRespawnPoint.rotation.eulerAngles.y, 0);
                    Undo.RecordObject(player, "Move Player to Spawnpoint");
                }
            }
        }
        
    }
}
