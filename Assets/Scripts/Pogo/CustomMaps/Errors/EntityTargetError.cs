using Pogo.CustomMaps.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Errors
{
    public class EntityTargetError : MapError
    {
        public EntityTargetError(
            Exception exception,
            EntityInstance entity,
            string key,
            string targetName,
            string failReason = null) : base(exception, GetMessage(entity, key, targetName, failReason), Severities.Error)
        {

        }

        private static string GetMessage(EntityInstance entity, string key, string targetName, string failReason = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(failReason ?? "See Exception");
            sb.Append("Key: ");
            sb.AppendLine(key);
            sb.Append("Target Name: ");
            sb.AppendLine(targetName);
            sb.Append("Entity: " + entity.ToString());

            return sb.ToString();
        }
    }
}
