using UnityEngine;

public interface IPlayerContinuousForce
{
    public bool IsValid();

    public Vector3 GetForce(PlayerController player, float deltaTime);

}