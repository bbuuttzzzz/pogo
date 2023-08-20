using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforms
{
    public interface IPlatformService
    {
        public string PersistentDataPath { get; }
        public void OnEnable();
        public void OnDestroy();
    }
}
