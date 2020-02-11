using ShapePainter.Shapes;
using ShapePainter.Utility;
using ShapePainter.Utility.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShapePainter {
    public partial class MainWindow : Window {
        private Dictionary<Group, List<CanvasShape>> objects = new Dictionary<Group, List<CanvasShape>>();
        private List<CanvasObject> selection = null;

        private Point? mouseDownPos = null;
        private CanvasShape selectionRect = null;

        private List<ICanvasCommand> history = new List<ICanvasCommand>();


        public MainWindow() {
            InitializeComponent();

            Add(Group.Global);

            // Test shapes
            Add(new CanvasShape(
                CloneShape.Clone(PlatonicForms.Ellipse),
                Group.Global,
                new Point(30, 30)
            ));

            Add(new CanvasShape(
                CloneShape.Clone(PlatonicForms.Rectangle),
                Group.Global,
                new Point(500, 300)
            ));

            Add(new CanvasShape(
                CloneShape.Clone(PlatonicForms.Ellipse),
                Group.Global,
                new Point(750, 700)
            ));
        }


        public void Add(CanvasObject obj) {
            // TODO: if selection contains ancestor, add this to selection.
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => {
                    objects.Add(group, new List<CanvasShape>());
                },
                (CanvasShape shape) => {
                    objects[(Group) shape.parent].Add(shape);

                    Canvas.Children.Add(shape.shape);
                    Canvas.SetLeft(shape.shape, shape.position.X);
                    Canvas.SetTop(shape.shape, shape.position.Y);
                },
                true
            );

            obj.accept(visitor);


            this.InvalidateVisual();
            Canvas.InvalidateVisual();
        }


        public void Remove(CanvasObject obj) {
            // TODO: If selection contains this or contains ancestor remove this from selection.
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => {
                    objects.Remove(group);
                },
                (CanvasShape shape) => { 
                    if (objects.ContainsKey((Group) shape.parent)) { 
                        objects[(Group) shape.parent].Remove(shape);
                    }

                    Canvas.Children.Remove(shape.shape);
                },
                true
            );

            obj.accept(visitor);


            this.InvalidateVisual();
            Canvas.InvalidateVisual();
        }


        public void Clear(object sender = null, EventArgs e = null) {
            foreach (var pair in objects) {
                foreach (var shape in pair.Value) {
                    var visitor = new CanvasObjectGenericVisitor(
                        (Group g) => { },
                        (CanvasShape s) => { Canvas.Children.Remove(s.shape); },
                        false
                    );
                }
            }

            objects.Clear();
        }


        public void New(object sender, EventArgs e) {
            
        }


        public void Save(object sender, EventArgs e) {

        }


        public void Open(object sender, EventArgs e) {

        }


        public void AddEllipse(object sender, EventArgs e) {

        }


        public void AddRectangle(object sender, EventArgs e) {

        }


        public void AddOrnament(object sender, EventArgs e) {

        }


        public void Select(Point min, Point max) {
            Deselect();

            List<CanvasObject> result = new List<CanvasObject>();

            foreach (var group in objects) {
                foreach (var shape in group.Value) {
                    // Select if the center is within the select area.
                    if (
                        shape.position.X >= min.X && shape.position.X <= max.X &&
                        shape.position.Y >= min.Y && shape.position.Y <= max.Y
                    ) result.Add(shape);
                }
            }

            selection = result;

            // Make selected elements red.
            foreach (var shape in result) ((CanvasShape) shape).shape.Stroke = Brushes.Red;
        }


        public void Select(Group group) {
            Deselect();

            selection = new List<CanvasObject> { group };

            // Make selected elements red.
            foreach (var shape in objects[group]) shape.shape.Stroke = Brushes.Red;
        }


        public void Deselect() {
            if (selection == null) return;

            // Turn previously selected elements back to being black.
            foreach (var obj in selection) {
                if (obj is CanvasShape) ((CanvasShape) obj).shape.Stroke = Brushes.Black;
            }

            selection = null;
        }


        public List<CanvasShape> GetSelection() {
            List<CanvasShape> result = new List<CanvasShape>();
            if (selection == null) return result;

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => {},
                (CanvasShape shape) => { result.Add(shape); },
                true
            );

            return result;
        }


        private void HandleMouseDown(object sender, MouseButtonEventArgs args) {
            mouseDownPos = args.GetPosition(Canvas);

            selectionRect = new CanvasShape(
                CloneShape.Clone(PlatonicForms.SelectionRectangle),
                Group.Global,
                mouseDownPos.Value
            );

            Add(selectionRect);
        }


        private void HandleMouseUp(object sender, MouseButtonEventArgs args) {
            if (mouseDownPos == null) return;

            Point mouseUpPos = args.GetPosition(Canvas);
            (Point min, Point max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mouseUpPos);

            Remove(selectionRect);

            Select(min, max);

            mouseDownPos = null;
        }


        private void HandleMouseMove(object sender, MouseEventArgs args) {
            if (selectionRect == null || mouseDownPos == null) return;

            Point mouseCurrentPos = Mouse.GetPosition(Canvas);
            (Point min, Point max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mouseCurrentPos);

            Canvas.SetLeft(selectionRect.shape, min.X);
            Canvas.SetTop(selectionRect.shape, min.Y);

            selectionRect.shape.Width  = max.X - min.X;
            selectionRect.shape.Height = max.Y - min.Y;
        }
    }
}
