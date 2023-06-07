using UnityEngine;

namespace WizardUtils
{
    [RequireComponent(typeof(Collider))]
    public class Sideshow : MonoBehaviour
    {
        public Collider Collider => collider; 
        new Collider collider;

        public Transform Target;

        void Awake()
        {
            collider = GetComponent<Collider>();
            if (Target == null)
            {
                throw new System.NullReferenceException($"Missing Target on Sideshow {gameObject}");
            }
        }
    }
}
