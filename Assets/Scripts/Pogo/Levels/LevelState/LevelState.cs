namespace Pogo.Levels
{
    [System.Serializable]
    public struct LevelState
    {
        public LevelDescriptor Level;
        public int State;

        public const int State_Any = -1;

        public bool AnyState
        {
            get => State == State_Any;
            set
            {
                if (value == AnyState) return;
                State = value ? State_Any : 0;
            }
        }
    }
}