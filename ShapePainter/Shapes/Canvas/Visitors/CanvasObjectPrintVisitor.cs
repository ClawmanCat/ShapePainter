using System;

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
            printedText += "{\"Group\"[{";

            foreach (CanvasObject obj in group.view()) obj.accept(this);

            printedText += "}]";
        }

        public void visit(CanvasShape obj)
        {
            //if die checkt of het ornament of shape is

            printedText += "\"shape\":/" obj.position.X; \"positiony\" \"width\" \"height\",";
        }

        public String getJSON()
        {
            return printedText;
        }
    }
}
