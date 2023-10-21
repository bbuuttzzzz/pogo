using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.BakingTools
{
    public class BakingObject : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool EnabledWhenBaking;
        public bool EnabledWhenNotBaking;
#endif
    }
}
