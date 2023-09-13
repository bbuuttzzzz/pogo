using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

namespace Pogo.Collectibles
{
    public class CollectibleController : MonoBehaviour
    {
        public AudioVolumeWaypointer AmbienceWaypointer;
        public GameObject RendererRoot;

        public UnityEvent OnInitializedUnCollected;
        public UnityEvent OnInitializedHalfCollected;
        public UnityEvent OnInitializedCollected;
        public UnityEvent OnCollected;

        private CollectibleStates currentState;
        private bool isCollected => currentState == CollectibleStates.Collected;

        private void Initialize(CollectibleStates state)
        {
            currentState = state;
            switch (currentState)
            {
                case CollectibleStates.Uncollected:
                    OnInitializedUnCollected?.Invoke();
                    break;
                case CollectibleStates.HalfCollected:
                    OnInitializedHalfCollected?.Invoke();
                    break;
                case CollectibleStates.Collected:
                    OnInitializedCollected?.Invoke();
                    break;
            }

            SetAmbienceWaypointer(true);
            RendererRoot.SetActive(!isCollected);
        }

        private void Collect()
        {
            currentState = CollectibleStates.Collected;
            SetAmbienceWaypointer(false);
            RendererRoot.SetActive(!isCollected);
        }

        private void SetAmbienceWaypointer(bool instant)
        {
            if (AmbienceWaypointer == null)
            {
                Debug.LogWarning("Missing AmbienceWaypointer on CollectibleController", this);
            }

            int targetState = isCollected ? 0 : -1;
            if (instant)
            {
                AmbienceWaypointer.SnapToWaypoint(targetState);
            }
            else
            {
                AmbienceWaypointer.GoToWaypoint(targetState);
            }
        }
    }
}
