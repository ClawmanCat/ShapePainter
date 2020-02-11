using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    public class CanvasShape : CanvasObject {
        [JsonIgnore]
        public Shape shape { get; set; }

        public CanvasShape(Shape shape, CanvasObject parent, Point position) : base(position, parent) {
            this.shape = shape;
        }

        public override void accept(ICanvasObjectVisitor visitor) {
            visitor.visit(this);
        }
    }
}
