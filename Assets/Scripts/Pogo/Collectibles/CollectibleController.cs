using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

namespace Pogo.Collectibles
{
    [RequireComponent(typeof(Trigger))]
    public class CollectibleController : MonoBehaviour
    {
        public CollectibleDescriptor Descriptor;

        public AudioVolumeWaypointer AmbienceWaypointer;
        public GameObject RendererRoot;
        public float RendererHideDelay;

        public UnityEvent OnInitializedUnCollected;
        public UnityEvent OnInitializedHalfCollected;
        public UnityEvent OnInitializedCollected;
        public UnityEvent OnCollected;

        private CollectibleStates currentState;
        private bool isCollected => currentState == CollectibleStates.Collected;

        private void Start()
        {
            GetComponent<Trigger>().OnActivated.AddListener(Trigger_OnActivated);

            // hide all collectibles in challenge mode
            if (PogoGameManager.PogoInstance.CurrentDifficulty == PogoGameManager.Difficulty.Challenge)
            {
                Initialize(CollectibleStates.Collected);
            }
            else
            {
                var state = CheckState();
                Initialize(state);
            }
        }

        private void Trigger_OnActivated()
        {
            if (isCollected) return;
            Collect();
        }

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
            StartCoroutine(DelayedHideRendererRoot(RendererHideDelay));
            OnCollected?.Invoke();
            PogoGameManager.PogoInstance.UnlockCollectible(Descriptor);
        }

        private IEnumerator DelayedHideRendererRoot(float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            RendererRoot.SetActive(false);
        }

        private CollectibleStates CheckState()
        {
            if (CollectedInSlot)
            {
                return CollectibleStates.Collected;
            }
            else return CollectibleStates.Uncollected;
        }

        private bool CollectedInSlot
        {
            get
            {
                if (PogoGameManager.PogoInstance.CurrentSlotDataTracker == null)
                {
                    return false;
                }

                var data = PogoGameManager.PogoInstance.CurrentSlotDataTracker.GetCollectible(Descriptor.Key);
                return data.isUnlocked;
            }
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
