using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardUtils;
using WizardUtils.GameSettings;

namespace Platforms
{
    public interface IPlatformService
    {
        public string PlatformURLName { get; }
        public string SaveDataPath { get; }
        public string PersistentDataPath { get; }
        public void OnEnable();
        public void OnDestroy();
        public IGameSettingService BuildGameSettingService(IEnumerable<GameSettingFloat> settings);
    }
}
