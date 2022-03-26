using System;
using UnityEngine;

namespace WizardUtils.Saving
{
    [Serializable]
    public class SaveValue
    {
        public SaveValueDescriptor Descriptor;
        public string StringValue;

        public SaveValue(SaveValue other)
        {
            Descriptor = other.Descriptor;
            StringValue = other.StringValue;
        }

        public SaveValue(SaveValueDescriptor descriptor)
        {
            Descriptor = descriptor;
            StringValue = descriptor.DefaultValue;
        }

        public SaveValue(SaveValueDescriptor descriptor, string value) : this(descriptor)
        {
            StringValue = value;
        }
    }
}
