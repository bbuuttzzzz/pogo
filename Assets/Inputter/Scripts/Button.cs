using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inputter
{
    public abstract class Button
    {
        public bool Released;
        public bool Pressed;
        public bool held;

        public string keySymbol; //a 0-3 char symbol that represents the respective character

        protected Button()
        {
            held = false;
            Pressed = false;
            Released = false;
        }

        public virtual void Reset(bool hard = false)
        {
            if (hard) held = false;
            Pressed = false;
            Released = false;
        }
        public abstract void Check();
    }
}
