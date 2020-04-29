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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Shapes;

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

        // For serialization
        public string shape_name {
            get { return shape is Ellipse ? "ellipse" : "rectangle"; }
            set {
                shape = (value == "ellipse")
                    ? PlatonicForms.BasicEllipse.Clone()
                    : PlatonicForms.BasicRectangle.Clone();
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

        // For serialization
        public double x { get { return position.X; } set { position = new Vector(value, position.Y); } }
        public double y { get { return position.Y; } set { position = new Vector(position.X, value); } }


        private bool backing_selected;
        [JsonIgnore] public bool selected {
            get { return backing_selected; }
            set {
                shape.StrokeDashArray = value
                    ? new DoubleCollection(new double[] { 4.0 })
                    : new DoubleCollection();

                // Animate selected objects.
                if (value) {
                    Timer timer = new Timer();
                    timer.Interval = 100;

                    timer.Elapsed += (object src, ElapsedEventArgs args) => {
                        if (Application.Current == null) {
                            timer.Stop();
                            return;
                        }

                        Application.Current.Dispatcher.Invoke(() => {
                            if (selected) {
                                shape.StrokeDashOffset += 0.01;
                                MainWindow.instance.InvalidateIfHas(this);
                            } else timer.Stop();
                        });
                    };

                    timer.Start();
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
                Debug.Assert(value.X >= 0 && value.Y >= 0, "Negative value passed to Shape.size.");

                shape.Width = value.X;
                shape.Height = value.Y;

                MainWindow.instance.InvalidateIfHas(this);
            }
        }

        // For serialization
        public double w { get { return size.X; } set { size = new Vector(value, size.Y); } }
        public double h { get { return size.Y; } set { size = new Vector(size.X, value); } }


        // A shape is real if it is actually a part of the canvas and not something like a selection rectangle.
        [JsonIgnore] public bool real { get; private set; }
        #endregion Properties


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
