using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils.GameSettings
{
    public interface IGameSettingService
    {
        public GameSettingFloat GetSetting(string key);
        public void Save();
    }
}
