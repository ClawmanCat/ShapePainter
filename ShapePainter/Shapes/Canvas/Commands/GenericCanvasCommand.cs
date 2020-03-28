using ShapePainter.Shapes.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes.Canvas {
    class GenericCanvasCommand : ICanvasCommand {
        public delegate void CommandDelegate(MainWindow window);

        private CommandDelegate doCmd, undoCmd;

        public GenericCanvasCommand(CommandDelegate doCmd, CommandDelegate undoCmd) {
            this.doCmd = doCmd;
            this.undoCmd = undoCmd;
        }

        public void doCommand(MainWindow window) {
            doCmd(window);
        }

        public void undoCommand(MainWindow window) {
            undoCmd(window);
        }
    }
}
