using UnityEngine;

namespace WizardUtils
{
    public class VersionReader : MonoBehaviour
    {
        public UnityStringEvent OnReadVersion;

        private void Awake()
        {
            OnReadVersion?.Invoke($"v{Application.version}");
        }
    }
}
