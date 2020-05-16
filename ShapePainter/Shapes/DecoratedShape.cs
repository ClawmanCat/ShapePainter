using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes {
    class DecoratedShape : ICanvasObject {
        public enum Side { LEFT, RIGHT, TOP, BOTTOM };

        public ICanvasObject obj { get; set; } = null;
        public string[] ornaments { get; set; } = new string[Enum.GetValues(typeof(Side)).Length];


        public DecoratedShape() : base() { }


        public DecoratedShape(ICanvasObject obj) : base(obj.parent, obj.children.ToArray()) {
            this.obj = obj;
        }


        public override void accept(IVisitor visitor) {
            obj.accept(visitor);
        }


        public override bool is_a(Type type) {
            return obj.GetType().IsAssignableFrom(type) || type == typeof(DecoratedShape);
        }
    }
}
