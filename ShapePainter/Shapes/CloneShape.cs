using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    public static class CloneShape {
        public static Shape Clone(Shape shape) {
            Shape result = (Shape)Activator.CreateInstance(shape.GetType());

            result.StrokeThickness = shape.StrokeThickness;
            result.Stroke = shape.Stroke;
            result.Fill = shape.Fill;
            result.Height = shape.Height;
            result.Width = shape.Width;
            result.HorizontalAlignment = shape.HorizontalAlignment;
            result.VerticalAlignment = shape.VerticalAlignment;

            return result;
        }
    }
}
