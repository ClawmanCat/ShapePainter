using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes {
    public class Group : ICanvasObject {
        public string name { get; set; }

        
        // For deserialization
        public Group() : base() { }


        public Group(string name, ICanvasObject parent, params ICanvasObject[] children) : base(parent, children) {
            this.name = name;
        }


        public override void accept(IVisitor visitor) {
            visitor.visit(this);

            if (visitor.recursive()) {
                foreach (var child in children) child.accept(visitor);
            }
        }
    }
}
