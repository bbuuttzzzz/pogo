using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace WizardUtils
{
    public class CheckboxController : MonoBehaviour
    {
        public GameObject CheckboxObject;

        public UnityFloatEvent OnToggleChangedFloat;
        public UnityBoolEvent OnToggleChanged;

        public float EnabledValue = 1;
        public float DisabledValue = 0;

        bool toggle;
        public bool Toggled
        {
            get => toggle;
            set
            {
                toggle = value;
                CheckboxObject?.gameObject.SetActive(toggle);
                OnToggleChanged?.Invoke(Toggled);
                OnToggleChangedFloat?.Invoke(ToggleAsFloat);
            }
        }

        public float ToggleAsFloat => Toggled ? EnabledValue : DisabledValue;

        public void Toggle()
        {
            Toggled = !Toggled;
        }

        public void SetToggledFromFloat(float toggledFloat)
        {
            Toggled = toggledFloat == EnabledValue;
        }
    }
}
