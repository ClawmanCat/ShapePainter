using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Utility.Command {
    public interface ICanvasCommand {
        void run(MainWindow window);
        void undo(MainWindow window);
    }
}
