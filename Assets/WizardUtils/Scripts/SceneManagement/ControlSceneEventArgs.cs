using System;

namespace WizardUtils.SceneManagement
{
    public class ControlSceneEventArgs : EventArgs
    {
        public readonly ControlSceneDescriptor InitialScene;
        public readonly ControlSceneDescriptor FinalScene;

        public ControlSceneEventArgs(ControlSceneDescriptor initialScene, ControlSceneDescriptor finalScene)
        {
            InitialScene = initialScene;
            FinalScene = finalScene;
        }

        public bool InControlScene => FinalScene != null;
    }
}
