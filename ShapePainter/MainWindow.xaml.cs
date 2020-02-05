using ShapePainter.Shapes;
using ShapePainter.Utility;
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
    using LocalizedShape = Localized<Shape>;
    using Selection = Variant<List<Localized<Shape>>, Group>;

    public partial class MainWindow : Window {
        private Dictionary<Group, List<LocalizedShape>> shapes = new Dictionary<Group, List<LocalizedShape>>();
        private Selection selection = null;

        private Point? mouseDownPos = null;
        private LocalizedShape selectionRect = null;


        public MainWindow() {
            InitializeComponent();

            // Test shapes
            Add(
                new LocalizedShape(
                    Shapes.Shapes.CloneShape(Shapes.Shapes.Ellipse),
                    new Point(30, 30)
                ),
                Group.Global
            );

            Add(
                new LocalizedShape(
                    Shapes.Shapes.CloneShape(Shapes.Shapes.Rectangle),
                    new Point(100, 100)
                ),
                Group.Global
            );

            Add(
                new LocalizedShape(
                    Shapes.Shapes.CloneShape(Shapes.Shapes.Ellipse),
                    new Point(500, 500)
                ),
                Group.Global
            );
        }


        public void Add(LocalizedShape shape, Group group) {
            if (!shapes.ContainsKey(group)) shapes.Add(group, new List<LocalizedShape>());

            shapes[group].Add(shape);

            Canvas.Children.Add(shape.value);
            Canvas.SetLeft(shape.value, shape.position.X);
            Canvas.SetTop(shape.value, shape.position.Y);

            this.InvalidateVisual();
            Canvas.InvalidateVisual();
        }


        public void Remove(LocalizedShape shape, Group group) {
            shapes[group].Remove(shape);
            Canvas.Children.Remove(shape.value);

            this.InvalidateVisual();
            Canvas.InvalidateVisual();
        }


        public void Clear(object sender = null, EventArgs e = null) {
            shapes.Clear();

            foreach (var group in shapes) {
                foreach (var shape in group.Value) Remove(shape, group.Key);
            }
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

            List<LocalizedShape> result = new List<LocalizedShape>();

            foreach (var group in shapes) {
                foreach (var shape in group.Value) {
                    // Select if the center is within the select area.
                    if (
                        shape.position.X >= min.X && shape.position.X <= max.X &&
                        shape.position.Y >= min.Y && shape.position.Y <= max.Y
                    ) result.Add(shape);
                }
            }

            selection = new Selection(result);

            // Make selected elements red.
            foreach (var shape in result) shape.value.Stroke = Brushes.Red;
        }


        public void Select(Group group) {
            Deselect();

            selection = new Selection(group);

            // Make selected elements red.
            foreach (var shape in shapes[group]) shape.value.Stroke = Brushes.Red;
        }


        public void Deselect() {
            if (selection == null) return;

            // Turn previously selected elements back to being black.
            foreach (var shape in (selection.contained is Group) ? shapes[selection] : selection) {
                shape.value.Stroke = Brushes.Black;
            }

            selection = null;
        }


        public List<LocalizedShape> GetSelection() {
            List<LocalizedShape> result = new List<LocalizedShape>();
            if (selection == null) return result;

            result.AddRange((selection.contained is Group) ? shapes[selection] : selection);
            return result;
        }


        private void HandleMouseDown(object sender, MouseButtonEventArgs args) {
            mouseDownPos = args.GetPosition(Canvas);

            selectionRect = new LocalizedShape(
                Shapes.Shapes.CloneShape(Shapes.Shapes.SelectionRectangle),
                mouseDownPos.Value
            );

            Add(selectionRect, Group.Global);
        }


        private void HandleMouseUp(object sender, MouseButtonEventArgs args) {
            if (mouseDownPos == null) return;

            Point mouseUpPos = args.GetPosition(Canvas);
            (Point min, Point max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mouseUpPos);

            Remove(selectionRect, Group.Global);

            Select(min, max);

            mouseDownPos = null;
        }


        private void HandleMouseMove(object sender, MouseEventArgs args) {
            if (selectionRect == null || mouseDownPos == null) return;

            Point mouseCurrentPos = Mouse.GetPosition(Canvas);
            (Point min, Point max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mouseCurrentPos);

            Canvas.SetLeft(selectionRect.value, min.X);
            Canvas.SetTop(selectionRect.value, min.Y);

            selectionRect.value.Width  = max.X - min.X;
            selectionRect.value.Height = max.Y - min.Y;
        }
    }
}
