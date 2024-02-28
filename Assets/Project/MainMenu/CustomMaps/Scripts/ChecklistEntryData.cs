using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps
{
    public struct ChecklistEntryData
    {
        public string Title;
        public bool IsRequired;
        public bool IsComplete;

        public bool AllowAutoCompleteWhenCompleted;
        public Action AutoCompleteAction;
        public string HintBody;
    }
}
