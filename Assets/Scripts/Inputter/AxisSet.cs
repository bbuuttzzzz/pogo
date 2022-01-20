using UnityEngine;

namespace Inputter
{
    public class AxisSet
    {
        Axis xAxis;
        int invertXAxis;
        Axis yAxis;
        int invertYAxis;
        Axis zAxis;
        int invertZAxis;


        Vector3 axisValue;
        public Vector3 AxisValue => axisValue;

        public AxisSet(Axis _xAxis = null,
                         Axis _yAxis = null,
                         Axis _zAxis = null)
        {
            xAxis = _xAxis;
            yAxis = _yAxis;
            zAxis = _zAxis;
        }

        public void Check()
        {
            float x = xAxis?.Check()?? 0;
            float y = yAxis?.Check()?? 0;
            float z = zAxis?.Check()?? 0;

            axisValue = new Vector3(x, y, z);
        }
    }
}
