using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes.Canvas {
    class ClearCommand : ICanvasCommand {
        private List<CanvasObject> objects, selection;

        public void doCommand(MainWindow window) {
            objects   = new List<CanvasObject>(window.objects);
            selection = new List<CanvasObject>(window.selection);

            window.ClearCanvas();
        }

        public void undoCommand(MainWindow window) {
            objects.ForEach(window.AddObject);
            window.SelectObjects(selection);
        }
    }
}
