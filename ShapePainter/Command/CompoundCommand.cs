using ShapePainter.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Command {
    public class CompoundCommand : ICanvasCommand {
        private List<ICanvasCommand> commands;

        public CompoundCommand(params ICanvasCommand[] commands) {
            this.commands = new List<ICanvasCommand>(commands);
        }

        public void doCommand(MainWindow window) {
            foreach (var command in commands) command.doCommand(window);
        }

        public void undoCommand(MainWindow window) {
            foreach (var command in commands.ReverseView()) command.undoCommand(window);
        }
    }
}
