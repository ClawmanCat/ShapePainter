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
        //get position which shape is close
        public static TextBox textbox(TextBox textBlock)
        {
            //textBlock.AppendText("Enter text");
            textBlock.AcceptsReturn = false;
            textBlock.TextWrapping = TextWrapping.Wrap;

            return textBlock;
        }
    }
}