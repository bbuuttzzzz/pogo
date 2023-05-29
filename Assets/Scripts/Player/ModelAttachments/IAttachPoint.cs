using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Players.Visuals.ModelAttachments
{
    public interface IAttachPoint
    {
        public void SnapAttachment(IPlayerAttachPointSnappable attachment);
    }
}
