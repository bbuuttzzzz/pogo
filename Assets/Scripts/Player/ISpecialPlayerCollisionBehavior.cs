using WizardPhysics;

public interface ISpecialPlayerCollisionBehavior
{
    public void Perform(PlayerController target, CollisionEventArgs args);
}
