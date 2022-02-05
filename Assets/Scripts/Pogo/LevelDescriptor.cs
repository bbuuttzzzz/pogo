using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "new level", menuName = "ScriptableObjects/LevelDescriptor", order = 1)]
public class LevelDescriptor : ScriptableObject
{
    public int SceneNumber;
}