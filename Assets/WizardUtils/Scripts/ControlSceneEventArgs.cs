using System;

namespace WizardUtils
{
    public class ControlSceneEventArgs : EventArgs
    {
        public readonly ControlSceneDescriptor InitialScene;
        public readonly ControlSceneDescriptor FinalScene;

        public ControlSceneEventArgs(ControlSceneDescriptor initialScene, ControlSceneDescriptor finalScene)
        {
            this.InitialScene = initialScene;
            this.FinalScene = finalScene;
        }

        public bool InControlScene => FinalScene != null;
    }
}
