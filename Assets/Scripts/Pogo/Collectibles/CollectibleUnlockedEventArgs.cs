namespace Pogo.Collectibles
{
    public class CollectibleUnlockedEventArgs
    {
        public readonly CollectibleDescriptor Collectible;
        public readonly bool OnlyUnlockedInFile;

        public CollectibleUnlockedEventArgs(CollectibleDescriptor collectible, bool onlyUnlockedInFile)
        {
            Collectible = collectible;
            OnlyUnlockedInFile = onlyUnlockedInFile;
        }
    }
}