using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Math
{
    public static class GeometryHelper
    {
        //https://cp-algorithms.com/geometry/circle-line-intersection.html
        public static Vector2[] IntersectCircleLine(StandardLine2D line, float radius)
        {
            const double EPS = 1e-9;

            double r = radius;
            double a = line.A;
            double b = line.B;
            double c = line.C;

            double x0 = -a * c / (a*a + b*b);
            double y0 = -b * c / (a*a + b*b);
            if (c*c > r*r * (a*a + b*b) + EPS)
            {
                return new Vector2[] { };
            }
            else if (System.Math.Abs(c * c - r * r * (a * a + b * b)) < EPS)
            {
                return new Vector2[] { new Vector2((float)x0, (float)y0) };
            }
            else
            {
                double d = r * r - c * c / (a * a + b * b);
                double mult = System.Math.Sqrt(d / (a * a + b * b));
                double ax, ay, bx, by;
                ax = x0 + b * mult;
                bx = x0 - b * mult;
                ay = y0 - a * mult;
                by = y0 + a * mult;

                return new Vector2[]
                {
                    new Vector2 ((float)ax, (float)ay),
                    new Vector2 ((float)bx, (float)by),
                };
            }
        }
    }
}
