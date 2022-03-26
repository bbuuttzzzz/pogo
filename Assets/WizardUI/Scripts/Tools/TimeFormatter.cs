using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace WizardUtils
{
    public class TimeFormatter : MonoBehaviour
    {
        public UnityStringEvent ReceiveFormat;

        public void FormatFloat(float value)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(value);
            string result;
            if (timeSpan.Hours == 0)
            {
                result = timeSpan.ToString("m\\:ss\\.fff");
            }
            else
            {
                result = timeSpan.ToString("h\\:mm\\:ss\\.fff");
            }
            ReceiveFormat?.Invoke(result);
        }
    }
}
