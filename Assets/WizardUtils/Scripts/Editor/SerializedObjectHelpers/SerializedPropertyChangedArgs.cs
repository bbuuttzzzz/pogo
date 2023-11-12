using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils.SerializedObjectHelpers
{
    public class SerializedPropertyChangedArgs<T>
    {
        public T OldValue;
        public T NewValue;

        public SerializedPropertyChangedArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
