using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapePainter.Strategy {
    public class ClickStrategySelect : IClickStrategy {
        public void OnMouseDown(Vector downpos) {
            var window = MainWindow.instance;
            var selector = window.selection_rectangle;

            selector.position = downpos;
            selector.size = new Vector(1, 1);

            window.AddCanvasObject(selector);
        }


        public void OnMouseMoved(Vector downpos, Vector currpos) {
            var window = MainWindow.instance;
            
            window.selection_rectangle.Reshape(downpos, currpos);
            
            window.ClearSelection();
            window.SetSelected(downpos, currpos, true);
        }


        public void OnMouseUp(Vector downpos, Vector uppos) {
            var window = MainWindow.instance;

            window.ClearSelection();
            window.SetSelected(downpos, uppos, true);

            window.RemoveCanvasObject(window.selection_rectangle);
            window.SetClickStrategy(new ClickStrategyIdle());
        }
    }
}
