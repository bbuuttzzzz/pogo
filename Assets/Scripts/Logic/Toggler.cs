using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Logic
{
    public class Toggler : MonoBehaviour
    {
        public bool initalState;
        [SerializeField]
        private bool isToggledOn;

        public bool CallEventsOnInitialStateSet;

        private void Awake()
        {
            if (CallEventsOnInitialStateSet)
            {
                isToggledOn = !initalState;
                IsToggledOn = initalState;
            }
            else
            {
                isToggledOn = initalState;
            }
        }

        public bool IsToggledOn
        {
            get => isToggledOn; set
            {
                if (isToggledOn == value) return;
                isToggledOn = value;

                if (value) OnToggledOn?.Invoke();
                else OnToggledOff?.Invoke();
            }
        }

        public UnityEvent OnToggledOn;
        public UnityEvent OnToggledOff;
    }
}
