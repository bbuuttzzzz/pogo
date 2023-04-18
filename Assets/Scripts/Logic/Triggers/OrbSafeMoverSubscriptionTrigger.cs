using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardPhysics;

[RequireComponent(typeof(Collider))]
public class OrbSafeMoverSubscriptionTrigger : MonoBehaviour
{
    public OrbSafeMover[] Movers;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        foreach(var mover in Movers)
        {
            mover.Subscribe(player.CollisionGroup);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        foreach (var mover in Movers)
        {
            mover.Unsubscribe(player.CollisionGroup);
        }
    }
}
