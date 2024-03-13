using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps
{
    public struct GenerateMapThumbnailResult
    {
        public enum ResultTypes
        {
            Success,
            FailureMissingEntity
        }

        public MapHeader MapHeader;
        public ResultTypes ResultType;
    }
}
