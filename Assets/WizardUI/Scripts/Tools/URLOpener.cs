using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace WizardUI
{
    public class URLOpener : MonoBehaviour
    {
        public void OpenUrl(string URL)
        {
            Application.OpenURL(URL);
        }
    }
}
