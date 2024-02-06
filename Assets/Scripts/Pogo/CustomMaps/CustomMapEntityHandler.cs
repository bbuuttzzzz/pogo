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
        public Action<BSPLoader.EntityCreatedCallbackData> SetupAction { get; private set; }

        public CustomMapEntityHandler(string entityName, Action<BSPLoader.EntityCreatedCallbackData> setupAction)
        {
            ClassName = entityName;
            SetupAction = setupAction;
        }
    }
}
