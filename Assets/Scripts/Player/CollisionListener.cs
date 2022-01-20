using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionListener : MonoBehaviour
{
    public UnityEvent OnCollisionEnter;
    public UnityEvent OnCollisionExit;

    private void OnTriggerEnter(Collider other)
    {
        OnCollisionEnter.Invoke();   
    }

    private void OnTriggerExit(Collider other)
    {
        OnCollisionExit.Invoke();
    }
}
