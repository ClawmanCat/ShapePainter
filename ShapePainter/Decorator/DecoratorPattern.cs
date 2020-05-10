using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShapePainter.Shapes.Canvas
{
    public class DecoratorPattern
    {
        public TextBox top;
        public TextBox bottom;
        public TextBox right;
        public TextBox left;

        //get position which shape is close
        public TextBox textbox(ICanvasObject obj, TextBox textBlock, Vector position, bool real = true)
        {
            textBlock.AppendText("Enter text");
            textBlock.AcceptsReturn = false;
            textBlock.TextWrapping = TextWrapping.Wrap;

            return textBlock;
        }
        public void addTextToShape(TextBox top, TextBox bottom, TextBox right, TextBox left)
        {
            //get current selected shape
            MainWindow.instance.GetSelection();
            //add text close to that shape
            this.top = top;
            this.bottom = bottom;
            this.right = right;
            this.left = left;
        }
    }
}