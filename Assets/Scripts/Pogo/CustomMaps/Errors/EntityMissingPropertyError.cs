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
            EntityInstance entity,
            string key) : base(exception, GetMessage(entity, key), Severities.Error)
        {

        }

        private static string GetMessage(EntityInstance entity, string key)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Missing Key: ");
            sb.AppendLine(key);
            sb.Append("Entity: " + entity.ToString());

            return sb.ToString();
        }
    }
}
