using Pogo.Difficulties;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct SaveSlotPreviewData
    {
        public Difficulty difficulty;
        public string name;
        public int CompletionPerMille;
        public long TotalMilliseconds;
        public int TotalDeaths;
        public int LastFinishedChapter;
        public int TotalCollectibles;
        public int CollectedCoins;
        public float[] ChapterProgresses;
    }
}
