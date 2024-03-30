using Pogo.CustomMaps.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Steam
{
    public class UpdateMapResult
    {
        public enum ResultTypes
        {
            Success,
            CheckLegalAgreement,
            Failure,
        }

        public MapHeader UpdatedHeader;
        public ResultTypes ResultType;
        public bool Success => ResultType == ResultTypes.Success;
        public string ErrorMessage;
    }
}
