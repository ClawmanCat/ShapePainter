using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapePainter.Utility {
    public static class Utility {
        public static (Point, Point) GetMinMax(Point a, Point b) {
            Point min = new Point(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y)
            );

            Point max = new Point(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y)
            );

            return (min, max);
        }
    }
}
