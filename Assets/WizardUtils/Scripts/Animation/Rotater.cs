using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public Vector3 Axis = Vector3.forward;
    [Tooltip("Degrees Per Second")]
    public float RotationSpeed = 120;

    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, Axis);
    }
}
