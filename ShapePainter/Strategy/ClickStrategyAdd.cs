using ShapePainter.Command;
using ShapePainter.Shapes;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ShapePainter.Strategy
{
    using WPFShape = System.Windows.Shapes.Shape;

    public class ClickStrategyAdd : IClickStrategy {
        private WPFShape shape;
        private Shape canvas_shape;


        public ClickStrategyAdd(WPFShape shape) : base() {
            this.shape = shape;
            this.canvas_shape = null;
        }

        public void OnMouseDown(Vector downpos) {
            var window = MainWindow.instance;

            window.ClearSelection();

            // If any of the selection keys are down (Shift, Ctrl, Alt) switch to the desired behaviour instead.
            Key[] keys = { Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt };
            if (keys.Any(x => window.IsKeyPressed(x))) {
                window.ClearAddButtonStates();

                var strategy = new ClickStrategyIdle();

                window.SetClickStrategy(strategy);
                strategy.OnMouseDown(downpos);

                return;
            }


            this.canvas_shape = new Shape(window.base_node, shape.Clone(), downpos);
            window.DoCommand(new AddRemoveCommand(this.canvas_shape, true));
        }


        public void OnMouseMoved(Vector downpos, Vector currpos) {
            if (canvas_shape != null) {
                canvas_shape.Reshape(downpos, currpos);
            }
        }


        public void OnMouseUp(Vector downpos, Vector uppos) {
            OnMouseMoved(downpos, uppos);
            if (canvas_shape.size == new Vector(0, 0)) canvas_shape.size = new Vector(shape.Width, shape.Height);

            this.canvas_shape = null;
        }
    }
}
