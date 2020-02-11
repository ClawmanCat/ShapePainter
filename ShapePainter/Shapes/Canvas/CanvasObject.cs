using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    public abstract class CanvasObject {
        public Point position { get; set; }
        public CanvasObject parent { get; set; }


        public CanvasObject(Point position, CanvasObject parent) {
            this.parent = parent;

            Point parentpos = (parent == null) ? new Point(0, 0) : parent.position;
            this.position = new Point(
                position.X + parentpos.X,
                position.Y + parentpos.Y
            );
        }


        bool ancestor(CanvasObject obj) {
            for (var current = parent; current != null; current = current.parent) {
                if (current == obj) return true;
            }

            return false;
        }


        public abstract void accept(ICanvasObjectVisitor visitor);
    }
}
