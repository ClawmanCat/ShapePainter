using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapePainter.Utility {
    public class Localized<T> {
        public T value;
        public Point position;


        public Localized(T value, Point position) {
            this.value = value;
            this.position = position;
        }
    }
}
