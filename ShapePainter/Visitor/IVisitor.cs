using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Visitor {
    public interface IVisitor {
        void visit(Group group);
        void visit(Shape shape);

        bool recursive();
    }
}
