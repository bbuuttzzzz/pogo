using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace WizardUtils
{
    public class IntegerFormatter : MonoBehaviour
    {
        public string Format = "{0}";

        public UnityStringEvent ReceiveFormat;

        public void FormatInt(int value)
        {
            string result =string.Format(Format,value.ToString());
            ReceiveFormat?.Invoke(result);
        }
    }
}
