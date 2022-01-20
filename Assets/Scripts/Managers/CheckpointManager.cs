using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    private PlayerController player;
    public PlayerController Player => player;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void RegisterPlayer(PlayerController player)
    {
        this.player = player;
    }

    public static bool TryRegisterRespawnPoint(Transform newRespawnPoint)
    {
        if (Instance == null)
        {
            Debug.LogWarning("Tried to register a RespawnPoint with no CheckpointManager");
            return false;
        }

        if (newRespawnPoint == Instance.RespawnPoint) return false;

        Instance.RespawnPoint = newRespawnPoint;

        return true;

    }

    public Transform RespawnPoint;
}