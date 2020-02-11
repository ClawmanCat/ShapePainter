using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes {
    public class CanvasObjectGenericVisitor : ICanvasObjectVisitor {
        public delegate void VisitGroup(Group x);
        public delegate void VisitShape(CanvasShape x);

        private VisitGroup groupVisitor;
        private VisitShape shapeVisitor;
        private bool recursive;


        public CanvasObjectGenericVisitor(VisitGroup group, VisitShape shape, bool recursive) {
            this.groupVisitor = group;
            this.shapeVisitor = shape;
            this.recursive = recursive;
        }

        public void visit(Group group) {
            groupVisitor(group);
        }

        public void visit(CanvasShape shape) {
            shapeVisitor(shape);
        }

        bool ICanvasObjectVisitor.recursive() {
            return recursive;
        }
    }
}
