using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes.Canvas {
    public class AddRemoveCommand : ICanvasCommand {
        public enum Mode { ADD, REMOVE }

        private CanvasObject obj;
        private Mode mode;

        public AddRemoveCommand(CanvasObject obj, Mode mode) {
            this.obj = obj;
            this.mode = mode;
        }

        public void doCommand(MainWindow window) {
            if (mode == Mode.ADD) AddObject(window); else RemoveObject(window);

        }

        public void undoCommand(MainWindow window) {
            if (mode == Mode.ADD) RemoveObject(window); AddObject(window);
        }


        private void AddObject(MainWindow window) {
            window.AddObject(obj);
        }

        private void RemoveObject(MainWindow window) {
            window.RemoveObject(obj);
        }
    }
}
