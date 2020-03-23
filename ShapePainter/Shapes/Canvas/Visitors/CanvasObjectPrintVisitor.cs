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

            printedText += "\n}]";
        }

        public void visit(CanvasShape obj)
        {
            //if die checkt of het ornament of shape is
            //string shapeWidth = obj.Width.ToString();
            //string shapeHeight = obj.Height.ToString();
            string shapeString = "\n\"shape\":";
            string shapePosX = obj.position.X.ToString();
            string shapePosY = obj.position.Y.ToString();
            string endComma = ",";

            //printedText += "\n\"shape\": posx; \"positiony\" \"width\" \"height\",";
            printedText += shapeString + " " + shapePosX + " " + shapePosY + endComma;
        }

        public String getJSON()
        {
            return printedText;
        }
    }
}
