using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneratedCheckpointTriggerEffect : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    private void Start()
    {
        var checkpoint = GetComponent<GeneratedCheckpoint>();
        checkpoint.OnActivated.AddListener(Checkpoint_OnActivated);
        var mesh = checkpoint.GetComponent<MeshFilter>().sharedMesh;
    }

    private void Checkpoint_OnActivated()
    {
        throw new NotImplementedException();
    }
}
