using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Command {
    public interface ICanvasCommand {
        void doCommand(MainWindow window);
        void undoCommand(MainWindow window);
    }
}
