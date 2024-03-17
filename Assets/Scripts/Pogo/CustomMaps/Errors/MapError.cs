using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Errors
{
    public class MapError
    {
        public enum Severities
        {
            Error,
            Warning
        }

        public Exception Exception { get; private set; }
        public string Message { get; private set; }
        public Severities Severity { get; private set; }

        public MapError(Exception exception, string message, Severities severity)
        {
            Message = message;
            Exception = exception;
            Severity = severity;
        }

        public string ToLogString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Type: ");
            sb.AppendLine(GetType().Name);
            sb.AppendLine(Message);
            if (Exception != null)
            {
                sb.Append("Exception: ");
                sb.AppendLine(Exception.ToString());
            }

            return sb.ToString();
        }
    }
}
