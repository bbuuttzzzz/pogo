using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public Vector3 localAxis = Vector3.forward;
    [Tooltip("Degrees Per Second")]
    public float RotationSpeed = 120;

    public bool RotateOnAwake = true;
    public bool Rotating { get; set; }

    private void Awake()
    {
        if (RotateOnAwake) Rotating = true;
    }

    private void Update()
    {
        if (Rotating)
        {
            transform.localRotation *= Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, localAxis);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Mesh arrowMesh = Resources.Load<Mesh>("Models/turnAxisIndicator");

        Quaternion localAxisRotation = Quaternion.LookRotation(localAxis, Vector3.up);
        Gizmos.color = Color.cyan;
        Gizmos.DrawMesh(arrowMesh,transform.position,transform.rotation * localAxisRotation, Vector3.one);
    }
}
