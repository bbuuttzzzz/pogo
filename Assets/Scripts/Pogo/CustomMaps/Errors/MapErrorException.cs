using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Errors
{
    public class MapErrorException : Exception
    {
        public MapError MapError { get; set; }

        public MapErrorException(MapError error) : base(error.Message)
        {
            MapError = error;
        }
        public MapErrorException(MapError error, Exception innerException) : base(error.Message, innerException)
        {
            MapError = error;
        }
    }
}
