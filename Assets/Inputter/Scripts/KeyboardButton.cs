using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Inputter
{
    public class KeyboardButton : Button
    {
        Key key;

        KeyControl keyControl;

        public KeyboardButton(Keyboard _keyboard, Key _key) : base()
        {
            key = _key;
            keyControl = _keyboard[key];
            keySymbol = KeySymbols.GetSymbol(key);
        }

        public override void Check()
        {
            held = keyControl.isPressed;
            Pressed = Pressed || keyControl.wasPressedThisFrame;
            Released = Released || keyControl.wasReleasedThisFrame;
        }
    }
}

