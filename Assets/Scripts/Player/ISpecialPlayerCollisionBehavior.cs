using WizardPhysics;

public interface ISpecialPlayerCollisionBehavior
{
    public bool TryOverrideCollisionBehavior(PlayerController target, CollisionEventArgs args, SurfaceConfig surfaceConfig);
}
