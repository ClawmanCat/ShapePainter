using Newtonsoft.Json;
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
        private Vector backing_position;
        public Vector position {
            get { return backing_position; }
            set {
                onPositionChanged(backing_position, value);
                backing_position = value;
            }
        }

        public CanvasObject parent { get; set; }


        public CanvasObject(Vector position, CanvasObject parent) {
            this.parent = parent;
            this.position = position;

            Vector parentpos = (parent == null) ? new Vector(0, 0) : parent.position;
            this.position += parentpos;
        }


        public bool ancestor(CanvasObject obj, bool self = false) {
            for (var current = self ? this : parent; current != null; current = current.parent) {
                if (current == obj) return true;
            }

            return false;
        }


        public abstract void accept(ICanvasObjectVisitor visitor);
        public virtual void onPositionChanged(Vector oldPos, Vector newPos) { }
    }
}
