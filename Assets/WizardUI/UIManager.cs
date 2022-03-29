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

        public void SpawnUIElement(GameObject prefab)
        {
            Instantiate(prefab, UIParent);
        }
    }
}
