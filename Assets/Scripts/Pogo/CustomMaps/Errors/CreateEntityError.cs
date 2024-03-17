using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Errors
{
    public class CreateEntityError : MapError
    {
        public CreateEntityError(Exception exception, EntityCreatedCallbackData data, string failReason = null) : base(exception, GetMessage(data, failReason), MapError.Severities.Error)
        {
            
        }

        private static string GetMessage(EntityCreatedCallbackData data, string failReason = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Reason: ");
            sb.AppendLine(failReason ?? "See Exception");
            sb.Append("Entity: " + data.Instance.entity.ToString());
            
            return sb.ToString();
        }
    }
}
