using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.MapSources
{
    public interface IMapSource
    {
        public bool AllowUpload { get; }
        public IEnumerable<MapLoadData> GetMaps();
    }
}
