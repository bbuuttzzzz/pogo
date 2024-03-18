using Pogo.CustomMaps.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Errors
{
    public class EntityBadFormatError : MapError
    {
        public EntityBadFormatError(
            Exception exception,
            EntityInstance entity,
            string key,
            string failReason = null) : base(exception, GetMessage(entity, key, failReason), Severities.Error)
        {

        }

        private static string GetMessage(EntityInstance entity, string key, string failReason = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(failReason ?? "See Exception");
            sb.Append("Malformed Key: ");
            sb.AppendLine(key);
            sb.Append("Entity: " + entity.ToString());

            return sb.ToString();
        }
    }
}
