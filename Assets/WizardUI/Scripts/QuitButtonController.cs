using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;

namespace WizardUtils
{
    public class QuitButtonController : MonoBehaviour
    {
        public GameObject HideThisObjectInWeb;

        private void Awake()
        {
#if UNITY_WEBGL
            HideThisObjectInWeb?.SetActive(false);
#endif
        }

        public void Quit()
        {
            GameManager.GameInstance?.Quit(true);
        }
    }
}
