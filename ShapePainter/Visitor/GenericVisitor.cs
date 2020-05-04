using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Visitor {
    public class GenericVisitor : IVisitor {
        public delegate void VisitGroup(Group group);
        public delegate void VisitShape(Shape shape);


        private VisitGroup group_visitor;
        private VisitShape shape_visitor;
        private bool is_recursive;


        public GenericVisitor(VisitGroup group, VisitShape shape, bool recursive) : base() {
            this.group_visitor = group;
            this.shape_visitor = shape;
            this.is_recursive  = recursive;
        }


        public void visit(Group group) {
            group_visitor(group);
        }

        public void visit(Shape shape) {
            shape_visitor(shape);
        }

        public bool recursive() {
            return is_recursive;
        }
    }
}
