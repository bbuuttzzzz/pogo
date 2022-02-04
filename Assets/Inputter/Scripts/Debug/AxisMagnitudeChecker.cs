using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Inputter.Debug
{
    public class AxisMagnitudeChecker : MonoBehaviour
    {
        public UnityEvent<float> OnAxisLengthChecked;

        public AxisSetName Axis;

        void Update()
        {
            Vector3 vec = InputManager.CheckAxisSet(Axis);
            OnAxisLengthChecked?.Invoke(vec.magnitude);
        }
    }
}