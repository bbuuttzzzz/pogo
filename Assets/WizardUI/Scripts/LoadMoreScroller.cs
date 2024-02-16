using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WizardUI
{
    [RequireComponent(typeof(ScrollRect))]
    public class LoadMoreScroller : MonoBehaviour
    {
        private float lastLoadMore;

        public float RepeatDelay = 0.1f;
        public UnityEvent OnShouldLoadMore;

        ScrollRect scrollRect;
        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            lastLoadMore = Time.unscaledTime - RepeatDelay;
        }

        private void Update()
        {
            if (lastLoadMore < Time.unscaledTime + RepeatDelay
                && scrollRect.verticalNormalizedPosition < 0.05f)
            {
                lastLoadMore = Time.unscaledTime;
                OnShouldLoadMore?.Invoke();
            }
        }
    }
}
