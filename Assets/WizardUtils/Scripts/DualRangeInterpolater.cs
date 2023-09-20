using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils
{
    public class DualRangeInterpolater
    {
        private readonly float ZeroValue;
        private readonly float OneValue;
        private readonly float TwoValue;

        public DualRangeInterpolater(float zeroValue, float oneValue, float twoValue)
        {
            ZeroValue = zeroValue;
            OneValue = oneValue;
            TwoValue = twoValue;
        }

        public float Interpolate(float t)
        {
            if (t < 1)
            {
                return Mathf.Lerp(ZeroValue, OneValue, t);
            }
            else
            {
                return Mathf.Lerp(OneValue, TwoValue, t - 1);
            }
        }
    }
}
