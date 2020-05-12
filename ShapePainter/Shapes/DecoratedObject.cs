using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Timers;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Reflection;
using ShapePainter.Utility;

namespace ShapePainter.Shapes {
    using WPFShape = System.Windows.Shapes.Shape;

    public class DecoratedObject : ICanvasObject {
        public enum Side { TOP, BOTTOM, LEFT, RIGHT };

        private ICanvasObject obj;
        private List<(Side, string)> text;

        public DecoratedObject(ICanvasObject contained) : base() {
            this.obj = contained;
        }

        public override void accept(IVisitor visitor) {
            obj.accept(visitor);
        }
    }
}