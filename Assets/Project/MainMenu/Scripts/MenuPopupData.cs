using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.MainMenu
{
    public struct MenuPopupData
    {
        public string Title;
        public string Body;

        public bool ShowOkButton;
        public string OkText;
        public Action OkPressedCallback;

        public bool ShowCancelButton;
        public string CancelText;
        public Action CancelPressedCallback;
    }
}
