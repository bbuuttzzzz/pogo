using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Saving
{
    [CreateAssetMenu(fileName = "sv_Value", menuName = "WizardUtils/Saving/SaveValueDescriptor", order = 1)]
    public class SaveValueDescriptor : ScriptableObject
    {
        public string Key;
        public string DefaultValue;
        public bool HideIfDefault = true;

        [TextArea(3,3)]
        public string DeveloperNote;
    }
}
