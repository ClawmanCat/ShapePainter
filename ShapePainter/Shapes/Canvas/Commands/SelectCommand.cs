using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes.Canvas {
    public class SelectCommand : ICanvasCommand {
        private List<CanvasObject> start, end;

        public SelectCommand() {}


        public void RecordStart(List<CanvasObject> selection) {
            start = selection;
        }


        public void RecordEnd(List<CanvasObject> selection) {
            end = selection;
        }


        public void doCommand(MainWindow window) {
            window.SelectObjects(end);
        }


        public void undoCommand(MainWindow window) {
            window.SelectObjects(start);
        }
    }
}
