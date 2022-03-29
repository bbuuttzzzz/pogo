using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupController : MonoBehaviour
{
    public UnityEvent OnOpened;

    public UnityEvent OnClosed;
    bool isClosed = true;

    public bool OpenOnAwake;

    [Tooltip("Last this many seconds. if 0, Lasts until manually closed")]
    public float Lifetime;

    public bool DestroyOnClosed;
    public float ClosedDestroyDelay;

    private void Awake()
    {
        Open();
    }

    public void Open()
    {
        if (!isClosed) return;
        isClosed = false;
        OnOpened?.Invoke();

        if (Lifetime != 0) StartCoroutine(TrackLifetime());
    }

    IEnumerator TrackLifetime()
    {
        yield return new WaitForSeconds(Lifetime);

        Close();
    }

    public void Close()
    {
        if (isClosed) return;
        Debug.Log("Closing");
        isClosed = false;
        OnClosed?.Invoke();
        if (DestroyOnClosed)
        {
            Destroy(gameObject, ClosedDestroyDelay);
        }
    }
}
