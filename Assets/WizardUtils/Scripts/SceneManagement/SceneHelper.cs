using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace WizardUtils.SceneManagement
{
    public static class SceneHelper
    {
        public static void DeactivateScene(Scene scene)
        {
            foreach(var gameObject in scene.GetRootGameObjects())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
