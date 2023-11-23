using Pogo.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pogo.AppearanceScreen
{
    [RequireComponent(typeof(Button))]
    public class PlayerRotaterController : MonoBehaviour
    {
        public AppearanceScreenController Parent;

        public float DragAmountScale = 1;

        private void Awake()
        {
            EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry startDragEntry = new EventTrigger.Entry();
            startDragEntry.eventID = EventTriggerType.InitializePotentialDrag;
            startDragEntry.callback.AddListener(Button_BeginDrag);
            trigger.triggers.Add(startDragEntry);

            EventTrigger.Entry whileDragEntry = new EventTrigger.Entry();
            whileDragEntry.eventID = EventTriggerType.Drag;
            whileDragEntry.callback.AddListener(Button_WhileDrag);
            trigger.triggers.Add(whileDragEntry);
        }

        private void Button_BeginDrag(BaseEventData baseEventData)
        {
            if (baseEventData is not PointerEventData pointerEventData)
            {
                return;
            }
            Parent.FakePlayer.StopRotating();
        }

        private void Button_WhileDrag(BaseEventData baseEventData)
        {
            if (baseEventData is not PointerEventData pointerEventData)
            {
                return;
            }
            float initialAngle = Parent.FakePlayer.CurrentRawAngle;
            float sizeAgnosticDelta = pointerEventData.delta.x / Screen.width;
            Parent.FakePlayer.SetRotationInstantly(initialAngle - sizeAgnosticDelta * DragAmountScale);
        }
    }
}
