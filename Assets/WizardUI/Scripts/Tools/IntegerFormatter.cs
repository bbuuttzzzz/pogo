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
        public string Format = "D";

        public UnityStringEvent ReceiveFormat;

        public void FormatInt(int value)
        {
            string result = value.ToString(Format);
            ReceiveFormat?.Invoke(result);
        }
    }
}
