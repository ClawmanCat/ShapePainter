using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapePainter.Utility {
    public static class Utility {
        public static (Vector, Vector) GetMinMax(Vector a, Vector b) {
            Vector min = new Vector(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y)
            );

            Vector max = new Vector(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y)
            );

            return (min, max);
        }
    }
}
