using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WizardUI
{
    public class ScrollEventListener : MonoBehaviour, IScrollHandler
    {
        public UnityEvent<PointerEventData> OnScrolled;

        public void OnScroll(PointerEventData eventData)
        {
            OnScrolled?.Invoke(eventData);
        }
    }
}
