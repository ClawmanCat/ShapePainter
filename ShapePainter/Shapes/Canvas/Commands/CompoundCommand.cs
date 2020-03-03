using ShapePainter.Shapes.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes.Canvas {
    public class CompoundCommand : ICanvasCommand {
        private IEnumerable<ICanvasCommand> commands;

        public CompoundCommand(IEnumerable<ICanvasCommand> commands) {
            this.commands = commands;
        }

        public void doCommand(MainWindow window) {
            foreach (ICanvasCommand command in commands) command.doCommand(window);
        }

        public void undoCommand(MainWindow window) {
            commands.Reverse();
            foreach (ICanvasCommand command in commands) command.undoCommand(window);
            commands.Reverse();
        }
    }
}
