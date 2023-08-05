using UnityEngine;

namespace WizardUI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public Transform UIParent;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            UIParent = UIParent ?? transform;
            Instance = this;
        }

        public GameObject SpawnUIElement(GameObject prefab)
        {
            return Instantiate(prefab, UIParent);
        }
    }
}
