﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapePainter.Shapes {
    public static class Shapes {
        public static readonly Shape Ellipse = new Ellipse {
            StrokeThickness = 4,
            Stroke = Brushes.Black,
            Fill = Brushes.OliveDrab,
            Height = 40,
            Width = 30,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };


        public static Shape CloneShape(Shape shape) {
            Shape result = (Shape) Activator.CreateInstance(shape.GetType());

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