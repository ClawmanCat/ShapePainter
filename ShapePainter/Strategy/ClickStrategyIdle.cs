using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapePainter.Strategy {
    public class ClickStrategyIdle : IClickStrategy {
        public void OnMouseDown(Vector downpos) {
            var window = MainWindow.instance;
            IClickStrategy strategy = null;

            if (window.IsKeyPressed(Key.LeftShift) || window.IsKeyPressed(Key.RightShift)) {
                strategy = new ClickStrategySelect();
            } else if (window.IsKeyPressed(Key.LeftCtrl) || window.IsKeyPressed(Key.RightCtrl)) {
                if (window.GetSelection().Count > 0) strategy = new ClickStrategyMove();
            } else if (window.IsKeyPressed(Key.LeftAlt) || window.IsKeyPressed(Key.RightAlt)) {
                if (window.GetSelection().Count == 1 && window.GetSelection()[0] is Shape) strategy = new ClickStrategyResize();
            }


            if (strategy != null) {
                window.SetClickStrategy(strategy);
                strategy.OnMouseDown(downpos);
            }
        }


        public void OnMouseMoved(Vector downpos, Vector currpos) {
            
        }


        public void OnMouseUp(Vector downpos, Vector uppos) {
            
        }
    }
}
