using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils.SceneManagement
{
    public class SceneLoadingData
    {
        public ControlSceneDescriptor InitialScene;
        public ControlSceneDescriptor FinalScene;
        public Action Callback;
    }
}
