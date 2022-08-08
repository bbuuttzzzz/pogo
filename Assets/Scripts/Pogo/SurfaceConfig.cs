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

    public (AudioClip clip, int index) NextRandomSound(int previousIndex = -1)
    {
        if (previousIndex >= 0 && previousIndex < Sounds.Length)
        {
            int index = Random.Range(0, Sounds.Length - 1);
            if (index >= previousIndex) index++;

            return (Sounds[index], index);
        }
        else
        {
            int index = Random.Range(0, Sounds.Length);
            return (Sounds[index], index);
        }
    }
}