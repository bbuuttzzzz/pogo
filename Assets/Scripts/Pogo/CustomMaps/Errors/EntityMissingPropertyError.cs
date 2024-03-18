using Pogo.CustomMaps.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Errors
{
    public class EntityMissingPropertyError : MapError
    {
        public EntityMissingPropertyError(
            Exception exception,
            EntityInstance instance,
            string key) : base(exception, GetMessage(instance, key), Severities.Error)
        {

        }

        private static string GetMessage(EntityInstance instance, string key)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Missing Key: ");
            sb.AppendLine(key);
            sb.Append("Entity: " + instance.entity.ToString());

            return sb.ToString();
        }
    }
}
