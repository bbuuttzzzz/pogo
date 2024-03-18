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
            EntityInstance instance,
            string key,
            string targetName,
            string failReason = null) : base(exception, GetMessage(instance, key, targetName, failReason), Severities.Error)
        {

        }

        private static string GetMessage(EntityInstance instance, string key, string targetName, string failReason = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(failReason ?? "See Exception");
            sb.Append("Key: ");
            sb.AppendLine(key);
            sb.Append("Target Name: ");
            sb.AppendLine(targetName);
            sb.Append("Entity: ");
            sb.Append(instance.entity.ClassName);
            sb.Append(" ");
            sb.Append(instance.entity.ToString());

            return sb.ToString();
        }
    }
}
