﻿using System;
using System.Windows;

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
         //string objGroup = group.ToString();
            string objGroup = "Group";

            printedText += "{\n" + objGroup + ":" + "[";

            foreach (CanvasObject obj in group.view()) obj.accept(this);

            printedText += "\n}]";
        }

        public void visit(CanvasShape obj)
        {
           
            string shape = obj.shape.ToString();
            MessageBox.Show(shape);
            string trimmedShape = "\n" + "'" + shape.Replace("System.Windows.Shapes.", "") + "'" + ":";

            string shapePosX = obj.position.X.ToString();
            string shapePosY = obj.position.Y.ToString(); 
            //size vector?
            string shapeWidth = obj.shape.Width.ToString();
            string shapeHeight = obj.shape.Height.ToString();
            string endComma = ",";

            //if else die checkt of het ornament of shape is
            if (trimmedShape.Contains("Rectangle") || trimmedShape.Contains("Ellipse"))
            {
                printedText += trimmedShape + " " + shapePosX + " " + shapePosY + " " + shapeWidth + " " + shapeHeight + endComma;
            }
            else if (trimmedShape.Contains("ornament"))
            {
                string text = "ornament tekst";
                printedText += shapePosX + " " + text + endComma;
            }

        }

        public String getJSON()
        {
            return printedText;
        }
    }
}
