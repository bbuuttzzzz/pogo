using Pogo.CustomMaps.Indexing;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Steam
{
    public class CreateMapResult
    {
        public PublishedFileId_t NewFileId;
        public bool Success;
        public string ErrorMessage;
    }
}
