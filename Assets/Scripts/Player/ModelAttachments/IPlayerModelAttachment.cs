
namespace Players.Visuals.ModelAttachments
{
    public interface IPlayerModelAttachment : IPlayerAttachPointSnappable
    {
        public void OnAttach(PlayerController parent);

        public string Name { get; }
    }

    public enum PlayerModelAttachPoints
    {
        // Don't fuck with the order!!! it'll fuck up all existing prefabs.
        Custom,
        Transform,
        Head,
        Back,
    }
}
