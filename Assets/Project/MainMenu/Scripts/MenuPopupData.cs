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

        public readonly bool ShowOkButton => !string.IsNullOrEmpty(OkText);
        public string OkText;
        public Action OkPressedCallback;

        public readonly bool ShowCancelButton => !string.IsNullOrEmpty(CancelText);
        public string CancelText;
        public Action CancelPressedCallback;
    }
}
