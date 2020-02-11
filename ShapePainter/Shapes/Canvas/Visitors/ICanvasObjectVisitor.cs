using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes {
    public interface ICanvasObjectVisitor {
        void visit(Group group);
        void visit(CanvasShape obj);
        bool recursive();
    }
}
