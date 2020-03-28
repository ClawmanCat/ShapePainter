using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapePainter.Shapes.Canvas {
    public class MoveCommand : ICanvasCommand {
        private List<(CanvasObject, Vector)> start, end;

        public MoveCommand() {}


        public void RecordStart(List<CanvasObject> selection) {
            start = new List<(CanvasObject, Vector)>(selection.Select((CanvasObject o) => (o, o.position)));
        }


        public void RecordEnd(List<CanvasObject> selection) {
            end = new List<(CanvasObject, Vector)>(selection.Select((CanvasObject o) => (o, o.position)));
        }


        public void doCommand(MainWindow window) {
            foreach (var (obj, pos) in end) {
                obj.position = pos;
                window.InvalidateObject(obj);
            }
        }


        public void undoCommand(MainWindow window) {
            foreach (var (obj, pos) in start) {
                obj.position = pos;
                window.InvalidateObject(obj);
            }
        }
    }
}
