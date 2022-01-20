using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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

    public static void RegisterPlayer(PlayerController player)
    {
        Instance.player = player;
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

    public static void KillPlayer()
    {
        Instance?.player.Die();
    }

    public Transform RespawnPoint;
}