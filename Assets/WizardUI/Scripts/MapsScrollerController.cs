using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WizardUI
{
    public class MapsScrollerController : MonoBehaviour
    {
        private float lastLoadMore;

        public float LoadMoreThreshold = 50f;
        public float RepeatDelay = 0.1f;
        public UnityEvent OnShouldLoadMore;

        public RectTransform Viewport;
        public RectTransform Content;

        public float ViewportHeight => Viewport.rect.height;
        public float ContentHeight => Content.rect.height;
        public float ContentPosition
        {
            get
            {
                return Content.anchoredPosition.y;
            }
            set
            {
                var temp = Content.anchoredPosition;
                temp.y = value;
                Content.anchoredPosition = temp;
            }
        }
        public float DistanceFromTop => ContentPosition;
        public float DistanceFromBottom => ContentHeight - DistanceFromTop - ViewportHeight;

        private void Awake()
        {
            lastLoadMore = Time.unscaledTime - RepeatDelay;
        }

        private void Update()
        {
            if (lastLoadMore < Time.unscaledTime + RepeatDelay
                && DistanceFromBottom < LoadMoreThreshold)
            {
                lastLoadMore = Time.unscaledTime;
                OnShouldLoadMore?.Invoke();
            }
        }
    }
}
