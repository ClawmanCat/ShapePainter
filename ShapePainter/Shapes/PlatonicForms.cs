using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    using WPFShape = System.Windows.Shapes.Shape;


    public static class PlatonicForms {
        // Make shapes cloneable so we can actually use the ones below.
        public static WPFShape Clone(this WPFShape shape) {
            WPFShape clone = (WPFShape) Activator.CreateInstance(shape.GetType());

            clone.StrokeThickness = shape.StrokeThickness;
            clone.Stroke = shape.Stroke;
            clone.Fill = shape.Fill;
            clone.Height = shape.Height;
            clone.Width = shape.Width;
            clone.HorizontalAlignment = shape.HorizontalAlignment;
            clone.VerticalAlignment = shape.VerticalAlignment;

            return clone;
        }


        public static readonly WPFShape BasicEllipse = new Ellipse { 
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Fill = Brushes.Transparent,
            Height = 50,
            Width = 50,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };


        public static readonly WPFShape BasicRectangle = new Rectangle { 
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Fill = Brushes.Transparent,
            Height = 50,
            Width = 50,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };


        public static readonly WPFShape SelectionRectangle = new Rectangle { 
            StrokeThickness = 2,
            Stroke = Brushes.Black,
            Fill = Brushes.Transparent,
            Height = 1,
            Width = 1,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
    }
}
