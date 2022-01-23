using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputter
{
    static class KeySymbols
    {
        //God help me
        public static string GetSymbol(Key key)
        {
            //values are taken from the KeyCode class and in the order they appear there
            //I skip all that wouldn't appear on a keyboard (exclamation mark? how do you press that q.q)
            //hope i don't regret that decision

            switch(key)
            {
                case Key.Backspace: return "Bk";
                case Key.Tab: return "Tab";
                case Key.Enter: return "Ent";
                case Key.Escape: return "Esc";
                case Key.Space: return "Spc";
                case Key.Comma: return ",";
                case Key.Minus: return "-";
                case Key.Period: return ".";
                case Key.Slash: return "/";
                case Key.Digit0: return "0";
                case Key.Digit1: return "1";
                case Key.Digit2: return "2";
                case Key.Digit3: return "3";
                case Key.Digit4: return "4";
                case Key.Digit5: return "5";
                case Key.Digit6: return "6";
                case Key.Digit7: return "7";
                case Key.Digit8: return "8";
                case Key.Digit9: return "9";
                case Key.Semicolon: return ";";
                case Key.Equals: return "=";
                case Key.LeftBracket: return "[";
                case Key.Backslash: return "\\";
                case Key.RightBracket: return "]";
                case Key.Backquote: return "`";
                case Key.A: return "A";
                case Key.B: return "B";
                case Key.C: return "C";
                case Key.D: return "D";
                case Key.E: return "E";
                case Key.F: return "F";
                case Key.G: return "G";
                case Key.H: return "H";
                case Key.I: return "I";
                case Key.J: return "J";
                case Key.K: return "K";
                case Key.L: return "L";
                case Key.M: return "M";
                case Key.N: return "N";
                case Key.O: return "O";
                case Key.P: return "P";
                case Key.Q: return "Q";
                case Key.R: return "R";
                case Key.S: return "S";
                case Key.T: return "T";
                case Key.U: return "U";
                case Key.V: return "V";
                case Key.W: return "W";
                case Key.X: return "X";
                case Key.Y: return "Y";
                case Key.Z: return "Z";
                case Key.Delete: return "Del";
                case Key.Numpad0: return "n0";
                case Key.Numpad1: return "n1";
                case Key.Numpad2: return "n2";
                case Key.Numpad3: return "n3";
                case Key.Numpad4: return "n4";
                case Key.Numpad5: return "n5";
                case Key.Numpad6: return "n6";
                case Key.Numpad7: return "n7";
                case Key.Numpad8: return "n8";
                case Key.Numpad9: return "n9";
                case Key.NumpadPeriod: return "n.";
                case Key.NumpadDivide: return "n/";
                case Key.NumpadMultiply: return "n*";
                case Key.NumpadMinus: return "n-";
                case Key.NumpadEnter: return "nEn";
                case Key.UpArrow: return "Up";
                case Key.DownArrow: return "Dn";
                case Key.RightArrow: return "Lef";
                case Key.LeftArrow: return "Rig";
                case Key.Insert: return "Ins";
                case Key.Home: return "Hom";
                case Key.End: return "End";
                case Key.PageUp: return "PUp";
                case Key.PageDown: return "PDn";
                case Key.F1: return "F1";
                case Key.F2: return "F2";
                case Key.F3: return "F3";
                case Key.F4: return "F4";
                case Key.F5: return "F5";
                case Key.F6: return "F6";
                case Key.F7: return "F7";
                case Key.F8: return "F8";
                case Key.F9: return "F9";
                case Key.F10: return "F10";
                case Key.F11: return "F11";
                case Key.F12: return "F12";
                case Key.NumLock: return "NLK";
                case Key.CapsLock: return "CAP";
                case Key.ScrollLock: return "SLK";
                case Key.RightShift: return "rSh";
                case Key.LeftShift: return "Shf";
                case Key.RightCtrl: return "rCt";
                case Key.LeftCtrl: return "Ctr";
                case Key.RightAlt: return "rAl";
                case Key.LeftAlt: return "Alt";
                case Key.LeftWindows: return "Win";
                case Key.RightWindows: return "rWn";
            }
            return " ";
        }
    }
}
