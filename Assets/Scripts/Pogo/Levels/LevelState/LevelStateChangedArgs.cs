namespace Pogo.Levels
{
    public class LevelStateChangedArgs
    {
        public readonly LevelState? OldState;
        public readonly LevelState NewState;
        public readonly bool Instant;

        public LevelStateChangedArgs(LevelState? oldState, LevelState newState, bool instantaneous)
        {
            OldState = oldState;
            NewState = newState;
            Instant = instantaneous;
        }
    }
}