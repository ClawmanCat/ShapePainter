using ShapePainter.Shapes;
using ShapePainter.Utility;
using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ShapePainter.Utility {
    public static class GroupViewBuilder {
        public static TreeView Make(ICanvasObject basenode) {
            Action<object, RoutedEventArgs, ICanvasObject, bool> mouse_callback = (object o, RoutedEventArgs e, ICanvasObject obj, bool selected) => {
                MainWindow.instance.OnGroupViewClicked(selected, obj);
                e.Handled = true;
            };


            Func<ICanvasObject, TreeViewItem> converter = null;
            converter = (ICanvasObject obj) => {
                TreeViewItem item = new TreeViewItem();

                item.Selected   += mouse_callback.Bind(true).Bind(obj).Invoke;
                item.Unselected += mouse_callback.Bind(false).Bind(obj).Invoke;
                item.IsExpanded = IsExpanded(obj);

                if (IsSelected(obj)) item.Background = Brushes.LightGray;


                if (obj is Shape) {
                    item.Header = ((Shape) obj).shape.GetType().Name;
                } else {
                    item.Header = "[G] " + ((Group) obj).name;
                    foreach (var child in obj.children) {
                        if (MainWindow.instance.HasCanvasObject(child)) item.Items.Add(converter(child));
                    }
                }


                return item;
            };


            TreeView view = new TreeView();
            view.Items.Add(converter(basenode));

            return view;
        }


        private static bool IsExpanded(ICanvasObject obj) {
            return MainWindow.instance.GetSelection().Contains(obj) || obj.children.Any(IsExpanded);
        }


        private static bool IsSelected(ICanvasObject obj) {
            return MainWindow.instance.GetSelection().Contains(obj);
        }
    }
}
