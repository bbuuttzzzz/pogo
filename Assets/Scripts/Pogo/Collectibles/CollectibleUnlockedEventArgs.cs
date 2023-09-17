namespace Pogo.Collectibles
{
    public class CollectibleUnlockedEventArgs
    {
        public readonly CollectibleDescriptor Collectible;
        public readonly bool UnlockedGlobally;
        public readonly bool UnlockedInSlot;

        public CollectibleUnlockedEventArgs(CollectibleDescriptor collectible, bool unlockedGlobally, bool unlockedInSlot)
        {
            Collectible = collectible;
            UnlockedGlobally = unlockedGlobally;
            UnlockedInSlot = unlockedInSlot;
        }
    }
}