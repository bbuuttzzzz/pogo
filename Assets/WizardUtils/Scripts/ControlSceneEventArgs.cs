using System;

namespace WizardUtils
{
    public class ControlSceneEventArgs : EventArgs
    {
        public readonly ControlSceneDescriptor ControlScene;

        public ControlSceneEventArgs(ControlSceneDescriptor controlScene)
        {
            ControlScene = controlScene;
        }

        public bool InControlScene => ControlScene != null;
    }
}
