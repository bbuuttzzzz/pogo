using Pogo;
using Pogo.Abilities;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class AbilityTrigger : MonoBehaviour
{
    public UnityEvent OnTriggered;
    public AbilityDescriptor Ability;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggered.Invoke();

        var player = PogoGameManager.PogoInstance.Player;
        var abilityObject = Instantiate(Ability.Prefab, player.RenderTransform);
        abilityObject.GetComponent<PlayerAbility>().Apply(player);
    }
}