using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.GameSettings;

namespace Platforms.Portable
{
    public class PortablePlatformService : IPlatformService
    {
        public string PersistentDataPath => Application.persistentDataPath;

        public void OnEnable()
        {

        }

        public void OnDestroy()
        {

        }

        public IGameSettingService BuildGameSettingService(IEnumerable<GameSettingFloat> settings)
        {
            return new PlayerPrefsGameSettingService(settings);
        }
    }
}
