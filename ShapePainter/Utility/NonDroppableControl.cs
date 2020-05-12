using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShapePainter.Utility {
    public static class NonDroppableControl {
        private static void AccessDrop(UIElement target, DragEventArgs args) {
            typeof(UIElement).GetMethod("OnDrop", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, new object[] { args });
        }


        public class NonDroppableTextBlock : TextBlock {
            protected override void OnDrop(DragEventArgs e) {
                AccessDrop(this.Parent as UIElement, e);
            }
        }


        public class NonDroppableTextBox : TextBox {
            protected override void OnDrop(DragEventArgs e) {
                AccessDrop(this.Parent as UIElement, e);
            }
        }
    }
}
