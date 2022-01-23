using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inputter
{
    public class DualButtonAxis : Axis
    {
        Button positive;
        Button negative;

        public DualButtonAxis(Button positive, Button negative)
        {
            this.positive = positive;
            this.negative = negative;
        }

        public override float Check()
        {
            return (positive.held ? 1 : 0)
                + (negative.held ? -1 : 0);
        }
    }
}
