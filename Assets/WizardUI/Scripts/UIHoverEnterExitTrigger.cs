using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WizardUI
{
    public class UIHoverEnterExitTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent<PointerEventData> OnHoverEnter;
        public UnityEvent<PointerEventData> OnHoverExit;

        public void OnPointerEnter(PointerEventData eventData) => OnHoverEnter?.Invoke(eventData);

        public void OnPointerExit(PointerEventData eventData) => OnHoverExit?.Invoke(eventData);
    }
}
