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

        public float Cooldown = 0;

        public bool CallEventsOnInitialStateSet;

        float lastActivation;
        private void Awake()
        {
            lastActivation = Time.time - Cooldown;
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
                if (Time.time < lastActivation + Cooldown)
                {
                    return;
                }
                lastActivation = Time.time;
                isToggledOn = value;

                if (value) OnToggledOn?.Invoke();
                else OnToggledOff?.Invoke();
            }
        }

        public void Toggle()
        {
            IsToggledOn = !IsToggledOn;
        }

        public UnityEvent OnToggledOn;
        public UnityEvent OnToggledOff;
    }
}
