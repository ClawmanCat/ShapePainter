using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    public static class PlatonicForms {
        public static readonly Shape Ellipse = new Ellipse {
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Fill = Brushes.Transparent,
            Height = 50,
            Width = 50,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };


        public static readonly Shape Rectangle = new Rectangle {
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Fill = Brushes.Transparent,
            Height = 50,
            Width = 50,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };


        public static readonly Shape SelectionRectangle = new Rectangle {
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
