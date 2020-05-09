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
using System.Windows.Media.Animation;

namespace ShapePainter.Shapes {
    using WPFShape = System.Windows.Shapes.Shape;

    public class Shape : ICanvasObject {
        #region Properties
        private WPFShape backing_shape;
        [JsonIgnore] public WPFShape shape {
            get { return backing_shape; }
            set {
                // Can't just invalidate since that would just update the old shape object.
                if (MainWindow.instance.HasCanvasObject(this)) {
                    MainWindow.instance.RemoveCanvasObject(this);
                    backing_shape = value;
                    MainWindow.instance.AddCanvasObject(this);
                } else {
                    backing_shape = value;
                }
            }
        }


        private Vector backing_position;
        [JsonIgnore] public Vector position {
            get { return backing_position; }
            set {
                backing_position = value;
                MainWindow.instance.InvalidateIfHas(this);
            }
        }


        private bool backing_selected;
        private Storyboard selection_animation_sb = new Storyboard();
        [JsonIgnore] public bool selected {
            get { return backing_selected; }
            set {
                shape.StrokeDashArray = value
                    ? new DoubleCollection(new double[] { 4.0 })
                    : new DoubleCollection();

                // Animate selected objects.
                if (value) {
                    var animation = new DoubleAnimation { From = 0, To = 70000, Duration = TimeSpan.FromSeconds(3600), RepeatBehavior = RepeatBehavior.Forever };
                    Storyboard.SetTarget(animation, shape);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(WPFShape.StrokeDashOffsetProperty));

                    selection_animation_sb.Children.Add(animation);
                    selection_animation_sb.Begin(shape);
                } else {
                    selection_animation_sb.Stop(shape);
                    selection_animation_sb.Children.Clear();
                }

                backing_selected = value;
                MainWindow.instance.InvalidateIfHas(this);
            }
        }


        // Size must be positive, use Reshape if negative values are used to indicate the direction of the shape has changed.
        [JsonIgnore] public Vector size {
            get {
                return new Vector(shape.Width, shape.Height);
            }
            set {
                shape.Width = value.X;
                shape.Height = value.Y;

                MainWindow.instance.InvalidateIfHas(this);
            }
        }


        // For serialization
        [JsonProperty(Order = 1)] public string shape_class {
            get { return (string) shape.Tag; }
            set {
                shape = typeof(PlatonicForms).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(x => x.GetValue(null))
                    .Where (x => x is WPFShape)
                    .Select(x => x as WPFShape)
                    .Where (x => value == x.Tag as string)
                    .First()
                    .Clone();
            }
        }

        [JsonProperty(Order = 2)] public double x { get { return position.X; } set { position = new Vector(value, position.Y); } }
        [JsonProperty(Order = 2)] public double y { get { return position.Y; } set { position = new Vector(position.X, value); } }
        [JsonProperty(Order = 2)] public double w { get { return size.X; } set { size = new Vector(value, size.Y); } }
        [JsonProperty(Order = 2)] public double h { get { return size.Y; } set { size = new Vector(size.X, value); } }


        // A shape is real if it is actually a part of the canvas and not something like the selection rectangle.
        [JsonIgnore] public bool real { get; private set; } = true;
        #endregion Properties


        // For deserialization
        public Shape() : base() { }


        public Shape(ICanvasObject parent, WPFShape shape, Vector position, bool real = true) : base(parent) {
            this.shape = shape;
            this.position = position;
            this.real = real;
        }


        public void Reshape(Vector a, Vector b) {
            var (min, max) = Utility.Utility.GetMinMax(a, b);

            this.position = min;
            this.size = max - min;
        }


        public override void accept(IVisitor visitor) {
            visitor.visit(this);
        }
    }
}
