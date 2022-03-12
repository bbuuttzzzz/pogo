using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Inputter
{
    public class MultiButton : Button
    {
        Button[] subButtons;

        public MultiButton(Button[] subButtons)
        {
            this.subButtons = subButtons;
        }

        public override void Check()
        {
            foreach(Button button in subButtons)
            {
                button.Check();
                held = held || button.held;
                Pressed = Pressed || button.Pressed;
                Released = Released || button.Released;
            }
        }

        public override void Reset(bool hard = false)
        {
            foreach (Button button in subButtons)
            {
                button.Reset();
            }
            base.Reset();
        }
    }
}
