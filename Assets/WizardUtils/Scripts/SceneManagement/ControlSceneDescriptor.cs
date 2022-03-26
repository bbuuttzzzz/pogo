using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WizardUtils.SceneManagement
{

    [CreateAssetMenu(fileName = "ControlSceneDescriptor", menuName = "WizardUtils/ControlSceneDescriptor", order = 1)]
    public class ControlSceneDescriptor : ScriptableObject
    {
        public int BuildIndex;
        public string ScenePath => SceneUtility.GetScenePathByBuildIndex(BuildIndex);
    }
}