using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils.Saving;

namespace WizardUtils.Saving
{
    [CreateAssetMenu(fileName = "ExplicitSaveData", menuName = "WizardUtils/Saving/ExplicitSaveData", order = 1)]

    public class ExplicitSaveData : ScriptableObject
    {
        public SaveManifest SaveManifest;

        public SaveValue[] Data;
    }
}
