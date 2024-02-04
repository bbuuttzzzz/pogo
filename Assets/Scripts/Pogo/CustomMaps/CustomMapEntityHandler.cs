using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps
{
    public class CustomMapEntityHandler
    {
        public string ClassName { get; private set; }
        public Action<BSPLoader.EntityInstance, List<BSPLoader.EntityInstance>> SetupAction { get; private set; }

        public CustomMapEntityHandler(string entityName, Action<BSPLoader.EntityInstance, List<BSPLoader.EntityInstance>> setupAction)
        {
            ClassName = entityName;
            SetupAction = setupAction;
        }
    }
}
