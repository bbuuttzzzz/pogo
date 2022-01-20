using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Inputter
{
    public class MouseButton : Button
    {
        ButtonControl control;

        private MouseButton(ButtonControl _control)
        {
            control = _control;
        }

        public static MouseButton LeftClick(Mouse _mouse)
        {
            return new MouseButton(_mouse.leftButton);
        }
        public static MouseButton RightClick(Mouse _mouse)
        {
            return new MouseButton(_mouse.rightButton);
        }

        public override void Check()
        {
            held = control.IsPressed();
            Pressed = Pressed || control.wasPressedThisFrame;
            Released = Released || control.wasReleasedThisFrame;
        }
    }
}
