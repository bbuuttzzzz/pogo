using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WizardUI
{
    public class ScrollPassThrough : MonoBehaviour, IScrollHandler
    {
        public IScrollHandler ParentScrollHandler;

        public void OnScroll(PointerEventData eventData)
        {
            ParentScrollHandler?.OnScroll(eventData);
        }
    }
}
