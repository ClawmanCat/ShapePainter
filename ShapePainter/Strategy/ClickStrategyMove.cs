using ShapePainter.Shapes;
using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapePainter.Strategy {
    public class ClickStrategyMove : IClickStrategy {
        private Vector? lastpos = null;


        public void OnMouseDown(Vector downpos) {
            lastpos = downpos;
        }


        public void OnMouseMoved(Vector downpos, Vector currpos) {
            if (lastpos == null) return;

            foreach (var item in MainWindow.instance.GetSelection()) {
                var visitor = new GenericVisitor(
                    (Group group) => {
                        // TODO
                    },
                    (Shape shape) => {
                        shape.position += (currpos - lastpos.Value);
                    },
                    false
                );

                item.accept(visitor);
            }

            lastpos = currpos;
        }


        public void OnMouseUp(Vector downpos, Vector uppos) {
            MainWindow.instance.SetClickStrategy(new ClickStrategyIdle());
        }
    }
}
