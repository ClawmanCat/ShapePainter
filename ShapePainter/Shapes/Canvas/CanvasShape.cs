using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    public class CanvasShape : CanvasObject {
        [JsonIgnore]
        public Shape shape { get; set; }
        public TextBox textBox { get; set; }


        public Vector size {
            get { return new Vector(shape.Width, shape.Height); }
            set {
                shape.Width  = value.X;
                shape.Height = value.Y;
            }
        }
        
        public bool selected {
            get { return shape.Stroke == Brushes.Red; }
            set { shape.Stroke = (value) ? Brushes.Red : Brushes.Black; }
        }

        public CanvasShape(Shape shape, CanvasObject parent, Vector position) : base(position, parent) {
            this.shape = shape;
        }
        public CanvasShape(TextBox textbox, CanvasObject parent, Vector position) : base(position, parent)
        {
            this.textBox = textbox;
        }

        public override void accept(ICanvasObjectVisitor visitor) {
            visitor.visit(this);
        }
    }
}
