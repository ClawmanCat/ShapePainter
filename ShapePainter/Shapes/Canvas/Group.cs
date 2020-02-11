using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapePainter.Shapes {
    public class Group : CanvasObject {
        public static Group Global = new Group(new Point(0, 0), null);

        private List<CanvasObject> contents = new List<CanvasObject>();


        public Group(Point position, CanvasObject parent) : base(position, parent) {}

        public void add(CanvasObject obj) {
            contents.Add(obj);
        }

        public void remove(CanvasObject obj) {
            contents.Remove(obj);
        }

        public IReadOnlyList<CanvasObject> view() {
            return contents;
        }

        public override void accept(ICanvasObjectVisitor visitor) {
            visitor.visit(this);

            if (visitor.recursive()) {
                foreach (CanvasObject obj in contents) obj.accept(visitor);
            }
        }
    }
}
