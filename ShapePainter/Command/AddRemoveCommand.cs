using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShapePainter.Command {
    public class AddRemoveCommand : ICanvasCommand {
        private ICanvasObject obj;
        private TextBox text;
        private bool add;


        public AddRemoveCommand(ICanvasObject obj, bool add) {
            this.obj = obj;
            this.add = add;
        }
        public AddRemoveCommand(TextBox obj, bool add)
        {
            this.text = obj;
            this.add = add;
        }


        public void doCommand(MainWindow window) {
            if (add) window.AddCanvasObject(obj);
            else window.RemoveCanvasObject(obj);
        }


        public void undoCommand(MainWindow window) {
            if (add) window.RemoveCanvasObject(obj);
            else window.AddCanvasObject(obj);
        }
    }
}
