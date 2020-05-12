using ShapePainter.Command;
using ShapePainter.Shapes;
using ShapePainter.Strategy;
using ShapePainter.Utility;
using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace ShapePainter {
    public partial class MainWindow : Window {
        // Singleton Object
        public static MainWindow instance { get; } = new MainWindow();

        #region ClassMembers
        [Resetable] private IClickStrategy click_strategy = new ClickStrategyIdle();
        [Resetable] private Vector mouse_down_pos = new Vector(0, 0);

        [Resetable] public Group base_node { get; set; } = null;

        [Resetable] public Shape selection_rectangle { get; private set; } = null;
        [Resetable] private List<ICanvasObject> selection = new List<ICanvasObject>();

        [Resetable] private List<ICanvasObject> objects = new List<ICanvasObject>();

        [Resetable] private List<ICanvasCommand> history = new List<ICanvasCommand>();
        [Resetable] private int history_pointer = 0;

        [Resetable] private HashSet<Key> keyboard = new HashSet<Key>();

        [Resetable] private bool update_group_view = true;

        private bool exit_on_close;

        TextBox textBlock;
        String textwrite = "";
        #endregion ClassMembers


        private MainWindow(bool exit_on_close = true) {
            InitializeComponent();

            this.exit_on_close = exit_on_close;
        }


        // Resets the object to its default state. 
        // Default values are obtained from a dummy object using reflection.
        public void Reset() {
            if (objects.Count > 0) DoCommand(new ClearCommand(objects));

            MainWindow tmp = new MainWindow(false);

            List<MemberInfo> fields = new List<MemberInfo>();
            fields.AddRange(this.GetType().GetRuntimeProperties().Cast<MemberInfo>());
            fields.AddRange(this.GetType().GetRuntimeFields().Cast<MemberInfo>());

            foreach (var field in fields) {
                if (!Attribute.IsDefined(field, typeof(Resetable))) continue;
                field.SetValue(this, field.GetValue(tmp));
            }

            this.base_node = new Group("Canvas", null);
            this.selection_rectangle = new Shape(null, PlatonicForms.SelectionRectangle.Clone(), new Vector(0, 0), false);

            this.EllipseButton.IsChecked = false;
            this.RectangleButton.IsChecked = false;
            this.OrnamentButton.IsChecked = false;

            this.Invalidate();
        }


        #region DoUndoRedo
        public void DoCommand(ICanvasCommand command, bool record = true) {
            if (record && history_pointer < history.Count) {
                history = history.GetRange(0, history_pointer);
            }

            command.doCommand(this);

            if (record) {
                history.Add(command);
                ++history_pointer;
            }
        }


        public void UndoCommand() {
            if (history_pointer < 1) return;

            var last = history[--history_pointer];
            last.undoCommand(this);
        }


        public void RedoCommand() {
            if (history_pointer >= history.Count) return;

            var next = history[history_pointer++];
            next.doCommand(this);
        }
        #endregion DoUndoRedo


        #region ObjectHandlers
         public void Invalidate() {
            foreach(var obj in objects) {
                var visitor = new GenericVisitor(
                    (Group group) => { },
                    (Shape shape) => {
                        Canvas.SetLeft(shape.shape, shape.position.X);
                        Canvas.SetTop(shape.shape, shape.position.Y);
                    },
                    false
                );

                obj.accept(visitor);
            }


            if (update_group_view) {
                this.GroupViewContainer.Content = GroupViewBuilder.Make(base_node, (TreeView) this.GroupViewContainer.Content);
                update_group_view = false;
            }


            this.InvalidateVisual();
            this.InvalidateArrange();
            this.InvalidateMeasure();
        }


        public void InvalidateIfHas(ICanvasObject obj) {
            if (HasCanvasObject(obj)) Invalidate();
        }


        public void AddCanvasObject(ICanvasObject obj) {
            var visitor = new GenericVisitor(
                (Group group) => {
                    this.objects.Add(group);
                },
                (Shape shape) => {
                    this.objects.Add(shape);
                    this.Canvas.Children.Add(shape.shape);

                    Canvas.SetLeft(shape.shape, shape.position.X);
                    Canvas.SetTop(shape.shape, shape.position.Y);
                },
                true
            );
            if (obj is DecoratedObject) { /* add text */ }
            obj.accept(visitor);
            update_group_view = true;
            Invalidate();
        }


        public void RemoveCanvasObject(ICanvasObject obj) {
            objects.Remove(obj);

            var visitor = new GenericVisitor(
                (Group group) => { },
                (Shape shape) => {
                    this.Canvas.Children.Remove(shape.shape);
                },
                true
            );
            if (obj is DecoratedObject) { /* remove text */ }
            obj.accept(visitor);
            update_group_view = true;
            Invalidate();
        }


        public bool HasCanvasObject(ICanvasObject obj) {
            return this.objects.Contains(obj);
        }


        public void SetSelected(ICanvasObject obj, bool selected) {
            if (obj is Shape && !((Shape) obj).real) return;

            Action<ICanvasObject> set_selection = (ICanvasObject x) => {
                if (selected) selection.Add(x);
                else selection.Remove(x);
            };

            var visitor = new GenericVisitor(
                (Group group) => {
                    set_selection(group);
                },
                (Shape shape) => {
                    shape.selected = selected;
                    set_selection(shape);
                },
                true
            );

            obj.accept(visitor);
            update_group_view = true;
            Invalidate();
        }


        public void SetSelected(IEnumerable<ICanvasObject> objs, bool selected) {
            foreach (var obj in objs) SetSelected(obj, selected);
        }


        public void SetSelected(Vector a, Vector b, bool selected) {
            var items = this.objects.Where(x => x is Shape && ((Shape) x).position.InBox(a, b));
            SetSelected(items, selected);
        }


        public void ClearSelection() {
            while (selection.Count > 0) SetSelected(selection[0], false);
        }
        #endregion ObjectHandlers


        #region PageHandlers
        private void OnKeyDown(object sender, KeyEventArgs e) {
            keyboard.Add(e.SystemKey == Key.None ? e.Key : e.SystemKey);

            // TODO: Clean this up into its own class if we have more keybinds.
            if (e.Key == Key.Escape) ClearSelection();
            if (e.Key == Key.Delete) DoCommand(new ClearCommand(selection));
        }


        private void OnKeyUp(object sender, KeyEventArgs e) {
            keyboard.Remove(e.SystemKey == Key.None ? e.Key : e.SystemKey);
        }


        private void OnMouseDown(object sender, MouseButtonEventArgs args) {
            Vector mousepos = (Vector) Mouse.GetPosition(this.Canvas);

            this.mouse_down_pos = mousepos;
            click_strategy.OnMouseDown(mousepos);
        }


        private void OnMouseUp(object sender, MouseButtonEventArgs args) { 
            var mousepos = (Vector) Mouse.GetPosition(this.Canvas);
            click_strategy.OnMouseUp(mouse_down_pos, mousepos);
        }


        private void OnMouseMoved(object sender, MouseEventArgs args) { 
            var mousepos = (Vector) Mouse.GetPosition(this.Canvas);
            click_strategy.OnMouseMoved(mouse_down_pos, mousepos);
        }


        private void OnNewButtonClicked(object sender, EventArgs e) {
            Reset();
        }


        private void OnOpenButtonClicked(object sender, EventArgs e) {
            Serializer.LoadJSON();
        }


        private void OnSaveButtonClicked(object sender, EventArgs e) {
            var result = WPFCustomMessageBox.CustomMessageBox.ShowYesNo(
                "Do you wish to save the current canvas as JSON data or export it to an image?",
                "Save to JSON or image?",
                "JSON",
                "Image",
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes) Serializer.SaveJSON();
            else Serializer.SaveBitmap();
        }
        
        
        private void OnClearButtonClicked(object sender, EventArgs e) {
            DoCommand(new ClearCommand(objects));
        }


        private void OnUndoButtonClicked(object sender, EventArgs e) {
            UndoCommand();
        }


        private void OnRedoButtonClicked(object sender, EventArgs e) {
            RedoCommand();
        }


        private void OnEllipseButtonClicked(object sender, EventArgs e) {
            RectangleButton.IsChecked = false;

            if (EllipseButton.IsChecked ?? false) SetClickStrategy(new ClickStrategyAdd(PlatonicForms.BasicEllipse));
            else SetClickStrategy(new ClickStrategyIdle());
        }


        private void OnRectangleButtonClicked(object sender, EventArgs e) {
            EllipseButton.IsChecked = false;

            if (RectangleButton.IsChecked ?? false) SetClickStrategy(new ClickStrategyAdd(PlatonicForms.BasicRectangle));
            else SetClickStrategy(new ClickStrategyIdle());
        }
        private void OnOrnamentButtonClicked(object sender, EventArgs e)
        {
            OrnamentButton.IsChecked = false;

            var selected = instance.GetSelection();
            //MessageBox.Show(selected.ToString());

            Point mousepos = Mouse.GetPosition(Canvas);
                textBlock = new TextBox();

                if (OrnamentButton.IsChecked ?? false) {
                    
                    MessageBox.Show("clicked ornament");
                    //SetClickStrategy(new ClickStrategyAdd(DecoratorPattern.textbox(textBlock)));
                }
            else SetClickStrategy(new ClickStrategyIdle());


            //DecoratorPattern.textbox(textBlock);

            //ornamentTextChange(sender, e);

            //RunCommand(new AddRemoveCommand(new CanvasShape(
            //  Decorator.textbox(textBlock),
            //  Group.Global,
            //  new Vector(mousepos.X, mousepos.Y)
            //  ), AddRemoveCommand.Mode.ADD));
        }
        private String ornamentTextChange(object sender, TextChangedEventArgs args)
        {
            textwrite += textBlock.Text;

            MessageBox.Show("text input =" + textBlock.Text);
            return textwrite;
        }

        private static int OnGroupViewAddClicked_Counter = 0;
        private void OnGroupViewAddClicked(object sender, EventArgs e) {
            Group group = new Group("Group " + OnGroupViewAddClicked_Counter++, base_node);

            DoCommand(new AddRemoveCommand(group, true));
            SetSelected(group, true);
        }


        private void OnGroupViewRemoveClicked(object sender, EventArgs e) {
            if (this.selection.Any(x => x != base_node)) {
                CompoundCommand cmd = new CompoundCommand(
                    this.selection
                        .Where(x => x != base_node)
                        .Select(x => new AddRemoveCommand(x, false))
                        .ToArray()
                );

                DoCommand(cmd);
            }
        }


        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            if (exit_on_close) Application.Current.Shutdown();
        }


        public void OnGroupViewClicked(bool selected, ICanvasObject obj) {
            // Ctrl + click = Add to selection, otherwise replace selection.
            if (!IsKeyPressed(Key.LeftCtrl) && !IsKeyPressed(Key.RightCtrl)) ClearSelection();
            SetSelected(obj, selected);
        }
        #endregion PageHandlers


        #region Interface
        public bool IsKeyPressed(Key key) {
            return keyboard.Contains(key);
        }


        public void SetClickStrategy(IClickStrategy strategy) {
            this.click_strategy = strategy;
        }


        public IReadOnlyList<ICanvasObject> GetSelection() {
            return selection;
        }


        public void ClearAddButtonStates() {
            this.EllipseButton.IsChecked = false;
            this.RectangleButton.IsChecked = false;
            this.OrnamentButton.IsChecked = false;
        }


        public RenderTargetBitmap ToBitmap() {
            var bitmap = new RenderTargetBitmap(
                (int) Canvas.RenderSize.Width, 
                (int) Canvas.RenderSize.Height, 
                100, 
                100, 
                PixelFormats.Pbgra32
            );

            bitmap.Render(this.Canvas);

            return bitmap;
        }
        #endregion Interface
    }
}
