using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Inputter
{
    public class AxisControlAxis : Axis
    {
        private AxisControl control;
        bool invert;

        private AxisControlAxis(AxisControl control, bool invert)
        {
            this.control = control;
            this.invert = invert;
        }

        public static AxisControlAxis MouseX(Mouse mouse, bool invert = false)
        {
            return new AxisControlAxis(mouse.delta.x, invert);
        }
        public static AxisControlAxis MouseY(Mouse mouse, bool invert = false)
        {
            return new AxisControlAxis(mouse.delta.y, invert);
        }

        public override float Check()
        {
            return control.ReadValue() * (invert ? -1 : 1);
        }
    }
}
