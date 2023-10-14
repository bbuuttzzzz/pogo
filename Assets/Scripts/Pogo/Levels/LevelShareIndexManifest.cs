using Pogo.Levels;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "levels_", menuName = "Pogo/LevelManifest", order = 1)]
public class LevelShareIndexManifest : ScriptableObject
{
    public LevelState[] LevelStates;
}