using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.PauseMenu
{
    public class PauseMenuFloaterButton
    {
        public Button Button;
        public Func<DisplayTypes> GetDisplayType;
        public Action OnActivated;

        public enum DisplayTypes
        {
            Enabled,
            Disabled,
            Hidden
        }
    }
}
