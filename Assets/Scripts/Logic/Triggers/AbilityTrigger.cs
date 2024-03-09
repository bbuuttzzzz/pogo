using Pogo;
using Pogo.Abilities;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class AbilityTrigger : MonoBehaviour
{
    public UnityEvent<Collider> OnTriggered;
    public AbilityDescriptor Ability;
    public Material DefaultMaterial;

    public float TriggerCooldown;

    private float LastTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerCooldown > 0 && LastTrigger + TriggerCooldown < Time.time)
        {
            return;
        }
        LastTrigger = Time.time;

        OnTriggered.Invoke(other);

        var player = PogoGameManager.PogoInstance.Player;
        var abilityObject = Instantiate(Ability.Prefab, player.RenderTransform);
        abilityObject.GetComponent<PlayerAbility>().Apply(player);
    }
}