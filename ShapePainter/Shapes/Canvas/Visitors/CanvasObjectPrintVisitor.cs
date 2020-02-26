using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes.Canvas.Visitors
{
    class CanvasObjectPrintVisitor : ICanvasObjectVisitor
    {
        String printedText = "";

        public bool recursive()
        {
            return false;
        }

        public void visit(Group group)
        {
            printedText += "{\"id\":naam,\"elements\": [";

            foreach (CanvasObject obj in group.view()) obj.accept(this);

            printedText += "]}";
        }

        public void visit(CanvasShape obj)
        {
            printedText += "{\"id:\"shape\"}";
        }

        public String getJSON()
        {
            return printedText;
        }
    }
}
