using ShapePainter.Shapes;
using ShapePainter.Shapes.Canvas;
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
using System.Windows.Shapes;

namespace ShapePainter {
    public partial class MainWindow : Window {
        private enum MouseState { NONE, SELECTING, MOVING }

        public List<CanvasObject> objects { get; set; }
        public List<CanvasObject> selection { get; set; }

        private List<ICanvasCommand> history = new List<ICanvasCommand>();

        private MouseState mouseState;
        private Vector? mouseDownPos = null, mousePrevPos = null;
        private CanvasShape selectionRectangle = null;

        // While we update the screen every time the mouse is moved, we only want to record the begin- and endstates
        // in the history.
        // To achieve this we keep a temporary command to record the begin and endstates in.
        ICanvasCommand recording;

        private Dictionary<Key, bool> keyboard = new Dictionary<Key, bool>();


        public MainWindow() {
            InitializeComponent();

            this.objects = new List<CanvasObject>();
            this.selection = new List<CanvasObject>();

            List<CanvasObject> initial = new List<CanvasObject> {
                Group.Global,
                new CanvasShape(CloneShape.Clone(PlatonicForms.Ellipse),   Group.Global, new Vector(30, 30)),
                new CanvasShape(CloneShape.Clone(PlatonicForms.Rectangle), Group.Global, new Vector(500, 300)),
                new CanvasShape(CloneShape.Clone(PlatonicForms.Ellipse),   Group.Global, new Vector(750, 700))
            };

            RunCommand(new CompoundCommand(initial.Select((CanvasObject o) => new AddRemoveCommand(o, AddRemoveCommand.Mode.ADD))));
        }


        public void RunCommand(ICanvasCommand command) {
            command.doCommand(this);
            history.Add(command);
        }


        public bool IsKeyDown(Key k) {
            return keyboard.ContainsKey(k) && keyboard[k];
        }


        private void SetKeyDown(Key k, bool down) {
            keyboard[k] = down;
        }


        public void AddObject(CanvasObject obj) {
            this.objects.Add(obj);

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => {
                    Canvas.Children.Add(shape.shape);
                    InvalidateObject(shape);
                },
                true
            );

            obj.accept(visitor);
            Invalidate();
        }


        public void RemoveObject(CanvasObject obj) {
            this.objects.Remove(obj);
            this.selection.Remove(obj);

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => {
                    Canvas.Children.Remove(shape.shape);
                },
                true
            );

            obj.accept(visitor);
            Invalidate();
        }




        public List<CanvasObject> GetSelectedObjects() {
            List<CanvasObject> result = new List<CanvasObject>();

            foreach (CanvasObject o in objects) {
                if (selection.Any((CanvasObject s) => o.ancestor(s, true))) result.Add(o);
            }

            return result;
        }


        private void Invalidate() {
            this.InvalidateVisual();
        }

        public void InvalidateObject(CanvasObject obj) {
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => {
                    Canvas.SetLeft(shape.shape, shape.position.X);
                    Canvas.SetTop(shape.shape,  shape.position.Y);
                },
                true
            );

            obj.accept(visitor);
            Invalidate();
        }


        public void SelectRegion(Vector a, Vector b, bool overwrite = true) {
            List<CanvasObject> result = new List<CanvasObject>();

            Vector min = new Vector(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
            Vector max = new Vector(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

            foreach (CanvasObject o in objects) {
                if (
                    o.position.X >= min.X && o.position.X < max.X &&
                    o.position.Y >= min.Y && o.position.Y < max.Y
                ) result.Add(o);
            }

            SelectObjects(result, overwrite);
        }


        #region Selection
        public void SelectObjects(IEnumerable<CanvasObject> objects, bool overwrite = true) {
            selection.Remove(selectionRectangle);   // Can't select the selection rectangle.

            if (overwrite) ClearSelection();

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => { shape.selected = true; },
                true
            );

            foreach (CanvasObject o in objects) o.accept(visitor);

            selection.AddRange(objects);
        }


        public void ClearSelection() {
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => { shape.selected = false; },
                true
            );

            foreach (CanvasObject o in selection) o.accept(visitor);

            this.selection.Clear();
        }


        public IReadOnlyList<CanvasObject> GetSelection() {
            return selection;
        }
        #endregion Selection


        #region PageHandlers
        public void ClearCanvas(object sender = null, EventArgs args = null) {
            foreach (var o in objects) RemoveObject(o);
            this.objects.Clear();
            
            this.ClearSelection();
        }


        public void New(object sender, EventArgs e) { }
        public void Save(object sender, EventArgs e) { }
        public void Open(object sender, EventArgs e) { }


        public void AddEllipse(object sender, EventArgs e) { }
        public void AddRectangle(object sender, EventArgs e) { }
        public void AddOrnament(object sender, EventArgs e) { }


        private void HandleMouseDown(object sender, MouseButtonEventArgs args) {
            this.mouseDownPos = (Vector) args.GetPosition(Canvas);
            this.mousePrevPos = mouseDownPos;

            // Drag to select, Shift + drag to move selection.
            mouseState = (IsKeyDown(Key.LeftShift) || IsKeyDown(Key.RightShift)) ? MouseState.MOVING : MouseState.SELECTING;

            // If selecting, create the selection rectangle.
            if (mouseState == MouseState.SELECTING) {
                this.selectionRectangle = new CanvasShape(
                    CloneShape.Clone(PlatonicForms.SelectionRectangle),
                    Group.Global,
                    mouseDownPos.Value
                );

                AddObject(this.selectionRectangle);

                recording = new SelectCommand();
                ((SelectCommand) recording).RecordStart(selection);
            } else if (mouseState == MouseState.MOVING) {
                recording = new MoveCommand();
                ((MoveCommand) recording).RecordStart(selection);
            }
        }


        private void HandleMouseUp(object sender, MouseButtonEventArgs args) {
            if (mouseState == MouseState.SELECTING) {
                RemoveObject(selectionRectangle);
                this.selectionRectangle = null;

                ((SelectCommand) recording).RecordEnd(selection);
            } else if (mouseState == MouseState.MOVING) {
                ((MoveCommand) recording).RecordEnd(selection);
            }

            this.mouseState = MouseState.NONE;
            this.mouseDownPos = null;
            this.mousePrevPos = null;

            if (recording != null) {
                history.Add(recording);
                recording = null;
            }
        }


        private void HandleMouseMove(object sender, MouseEventArgs args) {
            if (mouseState == MouseState.NONE) return;

            Vector mousePos = (Vector) args.GetPosition(Canvas);
            Vector delta = mousePos - mousePrevPos.Value;

            if (mouseState == MouseState.SELECTING) {
                // Update selection rect & select everything inside.
                SelectRegion(mouseDownPos.Value, mousePos);

                var (min, max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mousePos);
                selectionRectangle.position = min;
                selectionRectangle.size = max - min;

                InvalidateObject(selectionRectangle);
                Invalidate();
            } else {
                // Update position of all selected items.
                List<CanvasShape> shapes = new List<CanvasShape>();

                var visitor = new CanvasObjectGenericVisitor(
                    (Group group) => { },
                    (CanvasShape shape) => {
                        shapes.Add(shape);
                    },
                    true
                );

                foreach (var o in selection) o.accept(visitor);


                foreach (var shape in shapes) {
                    shape.position += delta;
                    InvalidateObject(shape);
                }
            }


            this.mousePrevPos = mousePos;
        }


        private void OnKeyDown(object sender, KeyEventArgs e) { SetKeyDown(e.Key, true);  }
        private void OnKeyUp(object sender, KeyEventArgs e)   { SetKeyDown(e.Key, false); }
        #endregion PageHandlers
    }
}
