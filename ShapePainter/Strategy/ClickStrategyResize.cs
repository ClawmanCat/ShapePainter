using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapePainter.Strategy {
    public class ClickStrategyResize : IClickStrategy {
        private Vector? old_pos = null;


        public void OnMouseDown(Vector downpos) {
            Shape shape = MainWindow.instance.GetSelection()[0] as Shape;

            old_pos = shape.position;
        }


        public void OnMouseMoved(Vector downpos, Vector currpos) {
            if (old_pos == null) return;

            Shape shape = MainWindow.instance.GetSelection()[0] as Shape;
            shape.Reshape(old_pos.Value, currpos);

        }


        public void OnMouseUp(Vector downpos, Vector uppos) {
            MainWindow.instance.SetClickStrategy(new ClickStrategyIdle());
        }
    }
}
