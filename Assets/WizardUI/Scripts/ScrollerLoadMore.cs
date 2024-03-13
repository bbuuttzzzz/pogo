using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace WizardUI
{
    public class ScrollerLoadMore : MonoBehaviour, IScrollHandler
    {
        private float lastLoadMore;

        public float LoadMoreThreshold = 50f;
        public float RepeatDelay = 0.1f;
        public UnityEvent OnShouldLoadMore;

        public float ScrollSensitivity = 0.1f;

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

        public float ScrollableHeight => ContentHeight - ViewportHeight;
        public float DistanceFromTop => ContentPosition;
        public float DistanceFromBottom => ScrollableHeight - DistanceFromTop;

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

        public void OnScroll(PointerEventData eventData)
        {
            ContentPosition -= eventData.scrollDelta.y * ScrollSensitivity;
            ContentPosition = Mathf.Clamp(ContentPosition, 0, ScrollableHeight);
        }
    }
}
