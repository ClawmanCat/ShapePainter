﻿using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Command {
    public class AddRemoveCommand : ICanvasCommand {
        private ICanvasObject obj;
        private bool add;


        public AddRemoveCommand(ICanvasObject obj, bool add) {
            this.obj = obj;
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
