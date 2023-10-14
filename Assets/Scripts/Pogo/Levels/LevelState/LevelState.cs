namespace Pogo.Levels
{
    [System.Serializable]
    public struct LevelState
    {
        public LevelDescriptor Level;
        public int StateId;

        public const int State_Any = -1;

        public bool AnyState
        {
            get => StateId == State_Any;
            set
            {
                if (value == AnyState) return;
                StateId = value ? State_Any : 0;
            }
        }
    }
}