

using Assets.Scripts.Pogo.Difficulty;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct SaveSlotPreviewData
    {
        public Difficulties difficulty;
        public string name;
        public int CompletionPerMille;
        public int TotalMilliseconds;
        public int TotalDeaths;
        public int LastFinishedChapter;
    }
}
