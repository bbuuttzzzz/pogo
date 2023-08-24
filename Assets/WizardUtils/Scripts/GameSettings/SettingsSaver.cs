using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.GameSettings
{
    public class SettingsSaver : MonoBehaviour
    {
        public void Save()
        {
            GameManager.GameInstance.SaveSettingsChanges();
        }
    }
}
