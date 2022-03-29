using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum KeyName
{
    none = -1, Jump,
    Forward, Right, Backward, Left,
    Pause,
    Reset,
    UIAdvance
}

public enum AxisSetName
{
    none = -1,
    Movement,
    Aim
}

namespace Inputter
{
    public class InputBinder
    {
        /*
         * NAMING CONVENTIONS
         * bool ___ - a button of that axis is currently pressed
         * bool ___Pressed - a button of that axis was just pressed
         * bool ___Released - a button of that axis was just released
         * bool ___Toggle - the state of a toggleable input, like a flashlight or similar
         * float ___Buffer - for when a button press is buffered. the game time in which the input should stop being buffered
         * float ___BufferMax - the time (seconds) that button should be buffered after it is pressed
         */
        public Vector3 moveVector;
        public Vector3 aimVector;

        int keycount;
        public Dictionary<KeyName, Button> Keys;
        public Dictionary<AxisSetName, AxisSet> AxisSets;

        public void Update()
        {
            GetInputs();
        }

        public InputBinder()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            keycount = System.Enum.GetNames(typeof(KeyName)).Length - 1; //-1 to account for "none" which is mapped to -1
            Keys = new Dictionary<KeyName, Button>();
            AxisSets = new Dictionary<AxisSetName, AxisSet>();

            Keys[KeyName.Jump] = new KeyboardButton(keyboard, Key.Space);

            Keys[KeyName.Forward] = new KeyboardButton(keyboard, Key.W);
            Keys[KeyName.Right] = new KeyboardButton(keyboard, Key.D);
            Keys[KeyName.Backward] = new KeyboardButton(keyboard, Key.S);
            Keys[KeyName.Left] = new KeyboardButton(keyboard, Key.A);

            Keys[KeyName.Reset] = new KeyboardButton(keyboard, Key.R);

#if UNITY_EDITOR
            Keys[KeyName.Pause] = new MultiButton(new Button[]{
                new KeyboardButton(keyboard, Key.Backquote),
                new KeyboardButton(keyboard, Key.P)
            });
#else
            Keys[KeyName.Pause] = new MultiButton(new Button[]{
                new KeyboardButton(keyboard, Key.Escape),
                new KeyboardButton(keyboard, Key.P)
            });
#endif
            Keys[KeyName.UIAdvance] = MouseButton.LeftClick(mouse);

            var movementAxisX = new DualButtonAxis(Keys[KeyName.Right], Keys[KeyName.Left]);
            var movementAxisY = new DualButtonAxis(Keys[KeyName.Forward], Keys[KeyName.Backward]);
            AxisSets[AxisSetName.Movement] = new AxisSet(_xAxis: movementAxisX, _zAxis: movementAxisY);

            var aimAxisX = AxisControlAxis.MouseX(mouse);
            var aimAxisY = AxisControlAxis.MouseY(mouse, true);
            AxisSets[AxisSetName.Aim] = new AxisSet(_xAxis: aimAxisY, _yAxis: aimAxisX);
        }
        public void ResetInputs(bool hard = false)
        {
            //reset all default inputs
            aimVector = Vector3.zero;

            if (hard)
            {
                moveVector = Vector3.zero;
            }

            for (int n = 0; n < keycount; n++)
            {
                KeyName nKey = (KeyName)n;
                Keys[nKey].Reset(hard);
            }
        }
        public void GetInputs()
        {
            for (int n = 0; n < keycount; n++)
            {
                Keys[(KeyName)n].Check();
            }

            foreach(KeyValuePair<AxisSetName, AxisSet> pair in AxisSets)
            {
                pair.Value.Check();
            }
        }
    }
}