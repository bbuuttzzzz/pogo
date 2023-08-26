

using Assets.Scripts.Pogo.Difficulty;

namespace Pogo.Saving
{
    [System.Serializable]
    public struct SaveSlotPreviewData
    {
        public Difficulties difficulty;
        public string name;
        public decimal Percentage;
        public decimal TotalTime;
        public int TotalDeaths;
        public int LastUnlockedChapter;
    }
}
