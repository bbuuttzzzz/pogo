using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new SurfaceConfig", menuName = "ScriptableObjects/SurfaceConfig", order = 1)]
public class SurfaceConfig : ScriptableObject
{
    public Material[] Materials;
    public AudioClip[] Sounds;

    public float SurfaceRepelForceMultiplier = 1;
    public float JumpForceMultiplier = 1;

    public AudioClip RandomSound => Sounds == null ? null
        : Sounds[Random.Range(0, Sounds.Length)];
}