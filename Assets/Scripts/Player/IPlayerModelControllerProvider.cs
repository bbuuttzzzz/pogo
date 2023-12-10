using UnityEngine.Events;

namespace Pogo.Cosmetics
{
    public interface IPlayerModelControllerProvider
    {
        public PlayerModelController PlayerModelController { get; }
        public UnityEvent<PlayerModelController> OnModelControllerChanged { get; }
    }
}
