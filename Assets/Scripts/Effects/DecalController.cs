using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalController : MonoBehaviour
{

    public void PerformEffect(Vector3 position, Vector3 normal)
    {
        transform.position = position;
        float angle = Mathf.Floor(Random.value * 4) % 4 * 90;
        transform.rotation = Quaternion.LookRotation(normal * -1, Vector3.up) * Quaternion.Euler(0,0,angle);
    }
}
